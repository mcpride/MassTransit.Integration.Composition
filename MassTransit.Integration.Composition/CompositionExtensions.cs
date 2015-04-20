// Copyright 2014 Marco Stolze
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using Magnum.Extensions;
using MassTransit.Saga;
using MassTransit.Saga.SubscriptionConfigurators;
using MassTransit.SubscriptionConfigurators;

namespace MassTransit.Integration.Composition
{
    public static class CompositionExtensions
    {
        public static void LoadFrom(this SubscriptionBusServiceConfigurator configurator, ExportProvider exportProvider)
        {
            LoadFrom(configurator, exportProvider, x => true);
        }

        public static void LoadFrom(this SubscriptionBusServiceConfigurator configurator, ExportProvider exportProvider, Predicate<Type> filter)
        {
            var concreteTypes = FindConsumerTypes(exportProvider, x => (!x.Implements<ISaga>() && filter(x))).ToList();
            if (concreteTypes.Any())
            {
                var consumerConfigurator = new CompositionConsumerFactoryConfigurator(configurator, exportProvider);

                foreach (var concreteType in concreteTypes)
                    consumerConfigurator.ConfigureConsumer(concreteType);
            }

            var sagaTypes = FindSagaTypes(exportProvider, x => filter(x)).ToList();
            if (sagaTypes.Any()) return;
            var sagaConfigurator = new CompositionSagaFactoryConfigurator(configurator, exportProvider);

            foreach (var type in sagaTypes)
                sagaConfigurator.ConfigureSaga(type);
        }

        public static ConsumerSubscriptionConfigurator<TConsumer> Consumer<TConsumer>(
            this SubscriptionBusServiceConfigurator configurator, ExportProvider exportProvider)
            where TConsumer : class, IConsumer
        {
            var consumerFactory = new CompositionConsumerFactory<TConsumer>(exportProvider);

            return configurator.Consumer(consumerFactory);
        }

        public static SagaSubscriptionConfigurator<TSaga> Saga<TSaga>(
            this SubscriptionBusServiceConfigurator configurator, ExportProvider exportProvider)
            where TSaga : class, ISaga
        {
            var sagaRepository = exportProvider.GetExportedValue<ISagaRepository<TSaga>>();

            var compositionSagaRepository = new CompositionSagaRepository<TSaga>(sagaRepository, exportProvider);

            return configurator.Saga(compositionSagaRepository);
        }

        private static IEnumerable<Type> FindConsumerTypes(ExportProvider exportProvider, Func<Type, bool> filter)
        {
            var exports = exportProvider.GetExports<IConsumer, IConsumerMetadata>();
            var results = new List<Type>();
            foreach (var type in exports
                .Select(contractType => contractType.Metadata.ContractType)
                .Where(filter)
                .Where(type => !results.Contains(type)))
            {
                results.Add(type);
            }
            return results;
        }

        private static IEnumerable<Type> FindSagaTypes(ExportProvider exportProvider, Func<Type, bool> filter)
        {
            var exports = exportProvider.GetExports<ISaga, ISagaMetadata>();
            var results = new List<Type>();
            foreach (var type in exports
                .Select(contractType => contractType.Metadata.ContractType)
                .Where(filter)
                .Where(type => !results.Contains(type)))
            {
                results.Add(type);
            }
            return results;
        }
    }
}
