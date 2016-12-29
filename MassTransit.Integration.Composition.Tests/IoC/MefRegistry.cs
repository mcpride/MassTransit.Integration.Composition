using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.Registration;
using System.Linq;
using MassTransit.Integration.Composition.Tests.Handlers;
using MassTransit.Integration.Composition.Tests.Repositories;
using MassTransit.Integration.Composition.Tests.Sagas;
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
                .SelectConstructor(
                    ctors => ctors.First(
                        info =>
                        {
                            var parameters = info.GetParameters();
                            if (parameters.Length != 1) return false;
                            return parameters[0].ParameterType == typeof (Guid);
                        }), (info, builder) => builder.AllowDefault())
                .Export<ISaga>(builder => builder.AddMetadata("ContractType", t => t));

            registration.ForType<TestSagaRepository>()
                .SetCreationPolicy(CreationPolicy.Shared)
                .Export<ISagaRepository<TestSaga>>();

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
