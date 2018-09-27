using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Web3Abstractions
{
    public interface IGetCode
    {
        Task<string> GetCode(string address);
    }
}
