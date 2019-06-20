using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing
{
    public interface IContractQuery
    {
        Task<object> Query(string contractAddress, string contractABI, string functionSignature, object[] functionInputs = null);
    }
}
