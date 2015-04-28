using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.Registration;
using MassTransit.Integration.Composition.Tests.Handlers;
using MassTransit.Saga;

namespace MassTransit.Integration.Composition.Tests.IoC
{
    public static class MefRegistry
    {
        public static RegistrationBuilder Registrate(this RegistrationBuilder registration)
        {
            registration.ForTypesDerivedFrom<TestHandlerBase>()
                .SetCreationPolicy(CreationPolicy.NonShared)
                .Export<IConsumer>(builder => builder
                    .AddMetadata("ContractType", t => t));

            registration.ForTypesDerivedFrom<ISaga>()
                .SetCreationPolicy(CreationPolicy.NonShared)
                .Export<ISaga>(builder => builder
                    .AddMetadata("ContractType", t => t));

            return registration;
        }

        public static ComposablePartCatalog GetCatalog(this RegistrationBuilder registration)
        {
            return new AggregateCatalog(
                new AssemblyCatalog(typeof (MefRegistry).Assembly, registration),
                new AssemblyCatalog(typeof (ISaga).Assembly)
            );
        }
    }
}
