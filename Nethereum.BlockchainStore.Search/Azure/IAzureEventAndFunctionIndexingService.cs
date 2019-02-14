using System.Threading.Tasks;
using Microsoft.Azure.Search.Models;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public interface IAzureEventAndFunctionIndexingService : 
        IAzureEventIndexingService, IAzureFunctionIndexingService
    {

    }
}