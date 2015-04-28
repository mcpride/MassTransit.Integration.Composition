using System;
using System.ComponentModel.Composition;

namespace MassTransit.Integration.Composition
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExportConsumerAttribute: ExportAttribute, IContractMetadata
    {
        public ExportConsumerAttribute(Type contractType)
            : base(AttributedModelServices.GetContractName(typeof(IConsumer)), contractType)
        {
        }
    }
}
