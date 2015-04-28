using System;
using Magnum.StateMachine;
using MassTransit.Integration.Composition.Tests.Messages;
using MassTransit.Saga;

namespace MassTransit.Integration.Composition.Tests.Sagas
{

    public class TestSaga : SagaStateMachine<TestSaga>, ISaga
    {
        static TestSaga()
        {
            Define(() =>
            {
                Correlate(Observation).By((saga, message) => saga.Name == message.Name);

                Initially(
                    When(Initiate)
                        .Then((saga, message) =>
                        {
                            saga.WasInitiated = true;
                            saga.Name = message.Name;
                        })
                        .TransitionTo(Initiated));

                During(Initiated,
                    When(Observation)
                        .Then((saga, message) => { saga.WasObserved = true; }),
                    When(Complete)
                        .Then((saga, message) => { saga.WasCompleted = true; })
                        .TransitionTo(Completed));
            });
        }

        //public TestSaga()
        //{
        //}

        //public TestSaga(Guid correlationId)
        //{
        //    CorrelationId = correlationId;
        //}

        public static State Initial { get; set; }
        public static State Completed { get; set; }
        public static State Initiated { get; set; }

        public static Event<InitiateSimpleSaga> Initiate { get; set; }
        public static Event<ObservableSagaMessage> Observation { get; set; }
        public static Event<CompleteSimpleSaga> Complete { get; set; }

        public bool WasInitiated { get; set; }
        public bool WasObserved { get; set; }
        public bool WasCompleted { get; set; }

        public string Name { get; set; }
        public Guid CorrelationId { get; set; }

        public IServiceBus Bus { get; set; }
    }
}
