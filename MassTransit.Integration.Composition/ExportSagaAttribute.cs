using System;
using System.ComponentModel.Composition;
using MassTransit.Saga;

namespace MassTransit.Integration.Composition
{
    public interface ISagaMetadata
    {
        Type ContractType { get; }
    }

    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExportSagaAttribute : ExportAttribute, ISagaMetadata
    {
        public ExportSagaAttribute(Type contractType)
            : base(AttributedModelServices.GetContractName(typeof(ISaga)), contractType)
        {
        }
    }
}
