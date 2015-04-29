// Copyright 2014, 2015 Marco Stolze
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
using MassTransit.Exceptions;
using MassTransit.Pipeline;

namespace MassTransit.Integration.Composition
{
    public class CompositionConsumerFactory<T> : IConsumerFactory<T> where T : class
    {
        private readonly ExportProvider _exportProvider;

        public CompositionConsumerFactory(ExportProvider exportProvider)
        {
            _exportProvider = exportProvider;
        }

        public IEnumerable<Action<IConsumeContext<TMessage>>> GetConsumer<TMessage>(
            IConsumeContext<TMessage> context, InstanceHandlerSelector<T, TMessage> selector) where TMessage : class
        {
            var exports = _exportProvider.GetExports<IConsumer, IContractMetadata>();
            var consumer = (from export in exports where export.Metadata.ContractType == typeof (T) select (T) export.Value).FirstOrDefault();
            if (consumer == null)
                throw new ConfigurationException(string.Format(StringResources.ErrorMessageUnableToResolveTypeFromServiceLocator(), typeof(T)));
            return selector(consumer, context);
        }
    }
}
