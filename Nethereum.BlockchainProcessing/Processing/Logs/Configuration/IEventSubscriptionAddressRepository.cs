using Nethereum.BlockchainProcessing.Processing.Logs;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Configuration
{
    public interface IEventSubscriptionAddressRepository
    {
        Task<IEventSubscriptionAddressDto[]> GetManyAsync(long eventSubscriptionId);
        Task<IEventSubscriptionAddressDto> UpsertAsync(IEventSubscriptionAddressDto dto);
    }
}
