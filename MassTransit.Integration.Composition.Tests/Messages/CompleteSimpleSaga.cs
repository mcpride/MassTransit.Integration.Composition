using System;

namespace MassTransit.Integration.Composition.Tests.Messages
{
    [Serializable]
    public class CompleteSimpleSaga :
        SimpleSagaMessageBase
    {
        public CompleteSimpleSaga()
        {
        }

        public CompleteSimpleSaga(Guid correlationId)
            :
                base(correlationId)
        {
        }
    }
}