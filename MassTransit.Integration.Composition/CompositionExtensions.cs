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
using System.ComponentModel.Composition;
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
            var concreteTypes = _findTypes(typeof(IConsumer), exportProvider, x => (!x.Implements<ISaga>() && filter(x))).ToList();
            if (concreteTypes.Any())
            {
                var consumerConfigurator = new CompositionConsumerFactoryConfigurator(configurator, exportProvider);

                foreach (var concreteType in concreteTypes)
                    consumerConfigurator.ConfigureConsumer(concreteType);
            }

            var sagaTypes = _findTypes(typeof(ISaga), exportProvider, x => filter(x)).ToList();
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

        private static Func<Type, ExportProvider, Func<Type, bool>, IEnumerable<Type>> _findTypes =
            (serviceType, exportProvider, filter) =>
            {
                var instances = exportProvider.GetExports<object>(AttributedModelServices.GetContractName(serviceType));
                var results = new List<Type>();
                foreach (var foundType in instances
                    .Select(instance => instance.GetType())
                    .Where(filter)
                    .Where(foundType => !results.Contains(foundType)))
                {
                    results.Add(foundType);
                }
                return results;
            };

        public static class Configure
        {
            public static void MethodToFindTypes(Func<Type, ExportProvider, Func<Type, bool>, IEnumerable<Type>> method)
            {
                _findTypes = method;
            }
        }
    }
}
