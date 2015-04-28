using System;

namespace MassTransit.Integration.Composition.Tests.Messages
{
    [Serializable]
    public class ObservableSagaMessage
    {
        public string Name { get; set; }
    }
}