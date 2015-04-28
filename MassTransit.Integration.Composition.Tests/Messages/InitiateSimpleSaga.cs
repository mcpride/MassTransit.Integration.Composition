using System;

namespace MassTransit.Integration.Composition.Tests.Messages
{
    [Serializable]
    public class InitiateSimpleSaga :
        SimpleSagaMessageBase
    {
        public InitiateSimpleSaga()
        {
        }

        public InitiateSimpleSaga(Guid correlationId)
            : base(correlationId)
        {
        }

        public string Name { get; set; }
    }
}