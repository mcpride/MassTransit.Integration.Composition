# MEF Composition integration for MassTransit

See also MassTransit http://masstransit-project.com

## Example: Usage with MEF

* Let your consumer or saga export the "IConsumer" contract name and its concrete contract type as metadata by using the "ExportConsumer" attribute. Also set the creation policy:

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


## Example for MEF fluent export instead of attributes usage:

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