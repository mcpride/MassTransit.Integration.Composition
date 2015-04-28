using System;
using System.ComponentModel.Composition;
using MassTransit.Saga;

namespace MassTransit.Integration.Composition
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExportSagaAttribute : ExportAttribute, IContractMetadata
    {
        public ExportSagaAttribute(Type contractType)
            : base(AttributedModelServices.GetContractName(typeof(ISaga)), contractType)
        {
        }
    }
}
