using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Nethereum.RPC.Eth.Blocks;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Web3Abstractions
{
    public interface IGetBlockWithTransactionHashesByNumber
    {
        Task<BlockWithTransactionHashes> GetBlockWithTransactionsHashesByNumber(long blockNumber);
    }
}
