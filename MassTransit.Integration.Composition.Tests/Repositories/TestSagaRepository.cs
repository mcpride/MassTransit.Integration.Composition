using MassTransit.Integration.Composition.Tests.Sagas;
using MassTransit.Saga;

namespace MassTransit.Integration.Composition.Tests.Repositories
{
    public class TestSagaRepository : InMemorySagaRepository<TestSaga>
    {
        public static ISagaRepository<TestSaga> Instance = null;

        public TestSagaRepository()
        {
            Instance = this;
        }
    }
}
