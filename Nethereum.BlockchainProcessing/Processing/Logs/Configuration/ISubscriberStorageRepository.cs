using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Configuration
{
    public interface ISubscriberStorageRepository
    {
        Task<ISubscriberStorageDto> GetAsync(long subscriberId, long repoId);
        Task<ISubscriberStorageDto[]> GetAsync(long subscriberId);
        Task<ISubscriberStorageDto> UpsertAsync(ISubscriberStorageDto dto);
    }
}
