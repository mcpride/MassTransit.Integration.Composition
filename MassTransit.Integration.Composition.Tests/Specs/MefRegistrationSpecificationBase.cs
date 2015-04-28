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
            var container = new CompositionContainer(new RegistrationBuilder().Registrate().GetCatalog());
            container.SatisfyImportsOnce(part);
            return container;
        }
    }
}