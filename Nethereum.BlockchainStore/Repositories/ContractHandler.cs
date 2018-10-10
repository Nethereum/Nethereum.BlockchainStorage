﻿using System.Threading.Tasks;
using Nethereum.BlockchainStore.Handlers;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Repositories
{
    public class ContractHandler : IContractHandler
    {
        private readonly IContractRepository _contractRepository;

        public ContractHandler(IContractRepository contractRepository)
        {
            this._contractRepository = contractRepository;
        }

        public async Task<bool> ExistsAsync(string contractAddress)
        {
            return await _contractRepository.ExistsAsync(contractAddress);
        }

        public async Task HandleAsync(ContractTransaction contractTransaction)
        {
            await _contractRepository.UpsertAsync(contractTransaction.ContractAddress, contractTransaction.Code, contractTransaction.Transaction);
        }
    }
}
