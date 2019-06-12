using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using System.Collections.Generic;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Configuration
{
    public interface IEventSubscriptionDto: IDbRow
    {
        bool CatchAllContractEvents { get; set; }
        long? ContractId { get; set; }
        bool Disabled { get; set; }
        List<string> EventSignatures { get; set; }
        long SubscriberId { get; set; }
    }
}