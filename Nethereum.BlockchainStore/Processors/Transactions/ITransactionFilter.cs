﻿using System.Collections.Generic;
using System.Linq;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Processors.Transactions
{
    public interface ITransactionFilter: IFilter<Transaction>
    {
    }
}