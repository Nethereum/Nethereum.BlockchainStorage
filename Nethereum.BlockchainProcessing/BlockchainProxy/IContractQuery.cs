using Nethereum.ABI.Model;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.BlockchainProxy
{
    public interface IContractQuery
    {
        Task<object> Query(string contractAddress, string contractABI, string functionSignature, object[] functionInputs = null);
    }
}
