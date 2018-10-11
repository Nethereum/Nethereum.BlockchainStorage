﻿using Nethereum.BlockchainStore.Handlers;
using System.Threading.Tasks;
using Nethereum.BlockchainStore.Processors;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Repositories
{
    public class TransactionHandler : ITransactionHandler
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAddressTransactionRepository _addressTransactionRepository;

        public TransactionHandler(
            ITransactionRepository transactionRepository, 
            IAddressTransactionRepository addressTransactionRepository = null)
        {
            _transactionRepository = transactionRepository;
            _addressTransactionRepository = addressTransactionRepository;
        }

        public async Task HandleContractCreationTransactionAsync(ContractCreationTransaction tx)
        {
            await _transactionRepository.UpsertAsync(
                tx.ContractAddress, 
                tx.Code, 
                tx.Transaction, 
                tx.TransactionReceipt, 
                tx.FailedCreatingContract, 
                tx.BlockTimestamp);

            await UpsertAddressTransactions(
                tx.Transaction, 
                tx.TransactionReceipt, 
                tx.FailedCreatingContract,
                tx.BlockTimestamp);
        }

        public async Task HandleTransactionAsync(TransactionWithReceipt tx)
        {
            await
                _transactionRepository.UpsertAsync(
                    tx.Transaction, 
                    tx.TransactionReceipt, 
                    tx.HasError, 
                    tx.BlockTimestamp, 
                    tx.HasVmStack, 
                    tx.Error);

                await UpsertAddressTransactions(
                    tx.Transaction, 
                    tx.TransactionReceipt, 
                    tx.HasError,
                    tx.BlockTimestamp,
                    tx.Error,
                    tx.HasVmStack);
        }

        private async Task UpsertAddressTransactions(
            Transaction tx, 
            TransactionReceipt receipt,
            bool hasError,
            HexBigInteger blockTimestamp, 
            string error = null, 
            bool hasVmStack = false)
        {
            if (_addressTransactionRepository == null) return;

            foreach (var address in tx.GetAllRelatedAddresses(receipt))
            {
                await _addressTransactionRepository.UpsertAsync(
                    tx,
                    receipt,
                    hasError, 
                    blockTimestamp, 
                    address, 
                    error, 
                    hasVmStack, 
                    receipt.ContractAddress);
            }

        }
    }
}
