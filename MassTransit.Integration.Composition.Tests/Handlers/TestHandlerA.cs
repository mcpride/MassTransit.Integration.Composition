using System.Threading;
using MassTransit.Integration.Composition.Tests.Messages;

namespace MassTransit.Integration.Composition.Tests.Handlers
{
    public class TestHandlerA : TestHandlerBase, Consumes<TestMessageA>.All
    {
        public void Consume(TestMessageA message)
        {
            Interlocked.Increment(ref Counter);
        }
    }
}
