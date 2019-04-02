using Newtonsoft.Json.Linq;

namespace Nethereum.BlockchainStore.Entities
{
    public interface ITransactionVmStackView
    {
        string Address { get;  }
        string StructLogs { get;  }
        string TransactionHash { get;  }
        JArray GetStructLogs();
    }
}