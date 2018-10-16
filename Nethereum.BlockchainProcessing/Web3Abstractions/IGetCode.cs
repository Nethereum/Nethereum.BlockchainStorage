using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Web3Abstractions
{
    public interface IGetCode
    {
        Task<string> GetCode(string address);
    }
}
