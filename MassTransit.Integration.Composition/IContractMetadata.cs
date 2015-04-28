using System;

namespace MassTransit.Integration.Composition
{
    public interface IContractMetadata
    {
        Type ContractType { get; }
    }
}