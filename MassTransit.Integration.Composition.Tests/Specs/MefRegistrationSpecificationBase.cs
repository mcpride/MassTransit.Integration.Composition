using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Registration;
using MS.QualityTools.UnitTestFramework.Specifications;
using MassTransit.Integration.Composition.Tests.IoC;

namespace MassTransit.Integration.Composition.Tests.Specs
{
    public abstract class MefRegistrationSpecificationBase : Specification
    {
        public ExportProvider GetExportProvider(object part)
        {
            var registry = new RegistrationBuilder().Registrate();
            var catalog = registry.GetCatalog();
            var container = new CompositionContainer(catalog);
            container.SatisfyImportsOnce(part);
            return container;
        }
    }
}