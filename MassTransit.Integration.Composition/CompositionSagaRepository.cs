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
using MassTransit.Pipeline;
using MassTransit.Saga;

namespace MassTransit.Integration.Composition
{
    public class CompositionSagaRepository<T> : ISagaRepository<T> where T : class, ISaga
    {
        private readonly ISagaRepository<T> _repository;
        private readonly ExportProvider _exportProvider;

        public CompositionSagaRepository(ISagaRepository<T> repository, ExportProvider exportProvider)
        {
            _repository = repository;
            _exportProvider = exportProvider;
        }

        public IEnumerable<Action<IConsumeContext<TMessage>>> GetSaga<TMessage>(
            IConsumeContext<TMessage> context, Guid sagaId, 
            InstanceHandlerSelector<T, TMessage> selector, ISagaPolicy<T, TMessage> policy) 
            where TMessage : class
        {
            return _repository.GetSaga(context, sagaId, selector, policy)
                .Select(consumer => (Action<IConsumeContext<TMessage>>) (x =>
                {
                    using (new CompositionContainer(_exportProvider))
                    {
                        consumer(x);
                    }
                }));
        }

        public IEnumerable<Guid> Find(ISagaFilter<T> filter)
        {
            return _repository.Find(filter);
        }

        public IEnumerable<T> Where(ISagaFilter<T> filter)
        {
            return _repository.Where(filter);
        }

        public IEnumerable<TResult> Where<TResult>(ISagaFilter<T> filter, Func<T, TResult> transformer)
        {
            return _repository.Where(filter, transformer);
        }

        public IEnumerable<TResult> Select<TResult>(Func<T, TResult> transformer)
        {
            return _repository.Select(transformer);
        }
    }
}