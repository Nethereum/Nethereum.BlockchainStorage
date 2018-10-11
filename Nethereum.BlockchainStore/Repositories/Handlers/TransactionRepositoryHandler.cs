﻿using System.Threading.Tasks;
using Nethereum.BlockchainStore.Handlers;
using Nethereum.BlockchainStore.Processors;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Repositories.Handlers
{
    public class TransactionRepositoryHandler : ITransactionHandler
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAddressTransactionRepository _addressTransactionRepository;

        public TransactionRepositoryHandler(
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
                tx.BlockTimestamp,
                contractAddress: tx.ContractAddress ?? tx.TransactionReceipt.ContractAddress);
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
            bool hasVmStack = false,
            string contractAddress = null)
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
                    contractAddress ?? receipt.ContractAddress);
            }

        }
    }
}
