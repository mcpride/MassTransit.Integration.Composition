# MEF Composition integration for MassTransit

See also MassTransit http://masstransit-project.com

## Consumers

### Example: Export consumers via attributes and load subscriptions

* Let your consumer export the "IConsumer" contract name and its concrete contract type as metadata by using the "ExportConsumer" attribute. Also set the creation policy:

```C#

[ExportConsumer(typeof(CustomerEventHandler))]
[PartCreationPolicy(CreationPolicy.NonShared)]
public class CustomerEventHandler : IEventHandler, 
    Consumes<CustomerCreated>.All, Consumes<CustomerChanged>.All, Consumes<CustomerRemoved>.All
{
    public void Consume(CustomerCreated @event)
    {
        Console.WriteLine("Customer with id '{0}' and name '{1}' has been created", @event.Id, @event.Name);
    }

    public void Consume(CustomerChanged @event)
    {
        Console.WriteLine("Customer with id '{0}' has been changed to '{1}'", @event.Id, @event.Name);
    }

    public void Consume(CustomerRemoved @event)
    {
        Console.WriteLine("Customer with id '{0}' has been removed", @event.Id);
    }
}

// just a marker interface used below for filtering
public interface IEventHandler
{
}

```

* Subscribe your consumers with the extension method magic with or without filters:

```C#

var bus = ServiceBusFactory.New(sbc =>
{
	sbc.UseMsmq(mqc =>
	{
		mqc.UseMulticastSubscriptionClient();
		mqc.VerifyMsmqConfiguration();
	});
	sbc.ReceiveFrom("msmq://localhost/sampleQueue");
    sbc.Subscribe(configurator => configurator.LoadFrom(compositionContainer, type => type.Implements<IEventHandler>()));
});

```

(in example: filtered by predicate: type => type.Implements...)


### Example: Export consumers via fluent interface

```C#

public static class MefRegistry
{
    public static RegistrationBuilder Registrate(this RegistrationBuilder registration)
    {
        registration.ForTypesDerivedFrom<TestHandlerBase>()
            .SetCreationPolicy(CreationPolicy.NonShared)
            .Export<IConsumer>(builder => builder
                .AddMetadata("ContractType", type => type));
        return registration;
    }

    public static ComposablePartCatalog GetCatalog(this RegistrationBuilder registration)
    {
        return new AssemblyCatalog(typeof (MefRegistry).Assembly, registration);
    }
}


...

var container = new CompositionContainer(new RegistrationBuilder().Registrate().GetCatalog());
...

```

## Sagas

The usage of sagas with the Microsoft Extensibility Framework in the manner of Masstransit is a little bit more complicated. Yo have to register the saga itself and a repository for this type of saga. 
Also you must give MEF a way to inject a Guid in a constructor, because Masstansit requires such a constructor signature for saga implementations. MEF doen't really use this but it has to know how it could be handled. One solution could be the following dummy implementation:

```C#

/// <summary>
/// Just a dummy helper for telling MEF that constructors with a Guid as parameter are handable.
/// </summary>
public class NewGuidFactory
{
    [Export(typeof(Guid))]
    public Guid NewGuid
    {
        get
        {
            return new Guid();
        }
    }
}

```

### Example: Export sagas via attributes

```C#

[ExportSaga(typeof(TestSaga))]
[PartCreationPolicy(CreationPolicy.NonShared)]
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

	public TestSaga(Guid correlationId)
	{
		CorrelationId = correlationId;
	}

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
	public Guid CorrelationId { get; private set; }

	public IServiceBus Bus { get; set; }
}


[Export(typeof(ISagaRepository<TestSaga>))]
[PartCreationPolicy(CreationPolicy.Shared)]
public class TestSagaRepository : InMemorySagaRepository<TestSaga>
{ }

```

### Example: Export sagas via fluent interface

```C#

registration.ForTypesDerivedFrom<ISaga>()
	.SetCreationPolicy(CreationPolicy.NonShared)
	.Export<ISaga>(builder => builder.AddMetadata("ContractType", t => t));

registration.ForType<TestSagaRepository>()
	.SetCreationPolicy(CreationPolicy.Shared)
	.Export<ISagaRepository<TestSaga>>();

```