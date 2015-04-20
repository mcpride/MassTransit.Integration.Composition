using System;
using System.ComponentModel.Composition;

namespace MassTransit.Integration.Composition
{
    public interface IConsumerMetadata
    {
        Type ContractType { get; }
    }

    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExportConsumerAttribute: ExportAttribute, IConsumerMetadata
    {
        public ExportConsumerAttribute(Type contractType)
            : base(AttributedModelServices.GetContractName(typeof(IConsumer)), contractType)
        {
        }
    }
}
