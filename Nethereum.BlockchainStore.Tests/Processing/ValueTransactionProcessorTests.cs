﻿using System;
using System.Threading.Tasks;
using Moq;
using Nethereum.BlockchainStore.Processors.Transactions;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Xunit;

namespace Nethereum.BlockchainStore.Tests.Processing
{
    public class ValueTransactionProcessorTests
    {
        private readonly Mock<ITransactionRepository> _mockTransactionRepository = new Mock<ITransactionRepository>();
        private readonly Mock<IAddressTransactionRepository> _mockAddressTransactionRepository = new Mock<IAddressTransactionRepository>();
        private readonly HexBigInteger _blockTimestamp = new HexBigInteger(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        private readonly Transaction _transaction = new Transaction { To = "0x1009b29f2094457d3dba62d1953efea58176ba27" };
        private readonly TransactionReceipt _transactionReceipt = new TransactionReceipt();

        private ValueTransactionProcessor CreateProcessor()
        {
            return new ValueTransactionProcessor(_mockTransactionRepository.Object, _mockAddressTransactionRepository.Object);
        }

        [Fact]
        public async Task ProcessTransactionAsync_Calls_TransactionRepo()
        {
            var processor = CreateProcessor();

            await processor.ProcessTransactionAsync(_transaction, _transactionReceipt, _blockTimestamp);

            _mockTransactionRepository
                .Verify(r => r.UpsertAsync(_transaction, _transactionReceipt, false, _blockTimestamp, false, null), 
                    Times.Once);

        }

        [Fact]
        public async Task ProcessTransactionAsync_Calls_AddressTransactionRepo()
        {

            var processor = CreateProcessor();

            await processor.ProcessTransactionAsync(_transaction, _transactionReceipt, _blockTimestamp);

            _mockAddressTransactionRepository
                .Verify(r => r.UpsertAsync(_transaction, _transactionReceipt, false, _blockTimestamp, _transaction.To, null, false, null), 
                    Times.Once);

        }
    }
}