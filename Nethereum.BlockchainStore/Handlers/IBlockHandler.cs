using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Handlers
{
    public interface IBlockHandler
    {
        Task Handle(BlockWithTransactionHashes block);
    }
}
