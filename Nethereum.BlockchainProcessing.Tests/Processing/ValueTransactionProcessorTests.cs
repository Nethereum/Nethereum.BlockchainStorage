using Moq;
using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.BlockchainProcessing.Processors.Transactions;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainStore.Tests.Processing
{
    public class ValueTransactionProcessorTests
    {
        private readonly Mock<ITransactionHandler> _mockTransactionRepository = new Mock<ITransactionHandler>();
        private readonly HexBigInteger _blockTimestamp = new HexBigInteger(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        private readonly Transaction _transaction = new Transaction { To = "0x1009b29f2094457d3dba62d1953efea58176ba27" };
        private readonly TransactionReceipt _transactionReceipt = new TransactionReceipt();

        private ValueTransactionProcessor CreateProcessor()
        {
            return new ValueTransactionProcessor(_mockTransactionRepository.Object);
        }

        [Fact]
        public async Task ProcessTransactionAsync_InvokesTransactionHandler()
        {
            var processor = CreateProcessor();

            await processor.ProcessTransactionAsync(_transaction, _transactionReceipt, _blockTimestamp);

            _mockTransactionRepository
                .Verify(r => r.HandleTransactionAsync(It.IsAny<TransactionWithReceipt>()), 
                    Times.Once);

        }
    }
}
