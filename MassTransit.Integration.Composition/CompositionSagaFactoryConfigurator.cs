// Copyright 2014-2017 Marco Stolze
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
using System.ComponentModel.Composition.Hosting;
using Magnum.Reflection;
using MassTransit.Saga;
using MassTransit.SubscriptionConfigurators;
using MassTransit.Util;

namespace MassTransit.Integration.Composition
{
    public class CompositionSagaFactoryConfigurator
    {
        private readonly SubscriptionBusServiceConfigurator _configurator;
        private readonly ExportProvider _exportProvider;

        public CompositionSagaFactoryConfigurator(SubscriptionBusServiceConfigurator configurator, ExportProvider exportProvider)
        {
            _configurator = configurator;
            _exportProvider = exportProvider;
        }

        public void ConfigureSaga(Type sagaType)
        {
            this.FastInvoke(new[] { sagaType }, "Configure");
        }

        [UsedImplicitly]
        public void Configure<T>()
            where T : class, ISaga
        {
            var sagaRepository = _exportProvider.GetExportedValue<ISagaRepository<T>>();
            _configurator.Saga(sagaRepository);
        }
    }
}
