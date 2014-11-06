using System.ComponentModel.Composition.Hosting;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions.Common;
using MassTransit.Integration.Composition.Tests.Handlers;
using MassTransit.Integration.Composition.Tests.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MS.QualityTools.UnitTestFramework.Specifications;

namespace MassTransit.Integration.Composition.Tests.Specs
{
    [TestClass]
    [SpecificationDescription("Use MassTransit consumers registered with MEF")]
    public class MassTransitHandlerRegistrationSpecification : MefRegistrationSpecificationBase
    {
        [TestMethod]
        [ScenarioDescription("Ensure automatic consumer registration")]
        public void EnsureAutomaticConsumerRegistration()
        {
            Given("a configured export provider with 2 registered TestMessageA handler types (consumers)", 
                testContext =>
                {
                    TestHandlerBase.Counter = 0;
                    testContext.State.Provider = GetExportProvider(this);
                } )
            .And("a configured MassTransit message bus", 
                testContext =>
                {
                    testContext.State.Bus = ServiceBusFactory.New(sbc =>
                    {
                        sbc.ReceiveFrom("loopback://localhost/queue");
                        sbc.Subscribe(configurator => configurator.LoadFrom((ExportProvider)testContext.State.Provider, type => type.Implements<TestHandlerBase>()));
                    });
                })
            .When("a TestMessageA message is published over MassTransit message bus", 
                testContext => Task.Factory
                    .StartNew(() => ((IServiceBus) testContext.State.Bus).Publish(new TestMessageA()))
                    .ContinueWith((task) => Thread.Sleep(1000)).Wait())
            .Then("both registered handlers should have consumed this message", testContext => TestHandlerBase.Counter == 2);
        }
    }
}
