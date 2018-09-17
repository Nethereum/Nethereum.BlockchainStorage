﻿using System.Threading.Tasks;
using Nethereum.BlockchainStore.Entities;
using Newtonsoft.Json.Linq;

namespace Nethereum.BlockchainStore.Repositories
{
    public interface ITransactionLogRepository
    {
        Task UpsertAsync(string transactionHash, long logIndex, JObject log);
        Task<ITransactionLogView> FindByTransactionHashAndLogIndexAsync(string hash, long idx);
    }
}