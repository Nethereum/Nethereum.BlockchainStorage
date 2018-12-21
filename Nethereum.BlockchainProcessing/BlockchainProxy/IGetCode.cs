using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.BlockchainProxy
{
    public interface IGetCode
    {
        Task<string> GetCode(string address);
    }
}
