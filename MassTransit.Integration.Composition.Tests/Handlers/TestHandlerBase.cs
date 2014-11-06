using System;

namespace MassTransit.Integration.Composition.Tests.Handlers
{
    public class TestHandlerBase: IConsumer
    {
        public static Int64 Counter = 0;
    }
}