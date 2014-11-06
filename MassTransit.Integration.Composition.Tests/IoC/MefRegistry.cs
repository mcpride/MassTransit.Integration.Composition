using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.Registration;
using MassTransit.Integration.Composition.Tests.Handlers;

namespace MassTransit.Integration.Composition.Tests.IoC
{
    public static class MefRegistry
    {
        public static RegistrationBuilder Registrate(this RegistrationBuilder registration)
        {
            registration.ForTypesDerivedFrom<TestHandlerBase>()
                .SetCreationPolicy(CreationPolicy.NonShared)
                .Export()
                .Export<IConsumer>();
            return registration;
        }

        public static ComposablePartCatalog GetCatalog(this RegistrationBuilder registration)
        {
            return new AssemblyCatalog(typeof (MefRegistry).Assembly, registration);
        }
    }
}
