using Moq;
using System.Threading.Tasks;
using Nethereum.BlockchainStore.Tests.Processing.ContractTransactionProcessorTests.Scenarios;
using Xunit;

namespace Nethereum.BlockchainStore.Tests.Processing.ContractTransactionProcessorTests
{
    public class IsTransactionForContractTests: ContractTransactionProcessorScenario
        {
            [Theory]
            [InlineData("")]
            [InlineData(null)]
            [InlineData("0x")]
            public async Task WhenToAddressIsEmpty_ReturnsFalse(string toAddress)
            {
                _transaction.To = toAddress;
                InitProcessor();

                Assert.False(await _processor.IsTransactionForContractAsync(_transaction));
            }

            [Fact]
            public async Task WhenToAddressIsNotInContractRepo_ReturnsFalse()
            {
                _transaction.To = "0x1009b29f2094457d3dba62d1953efea58176ba27";
                InitProcessor();
                _contractHandler.Setup(r => r.ExistsAsync(_transaction.To)).ReturnsAsync(false);

                Assert.False(await _processor.IsTransactionForContractAsync(_transaction));
            }

            [Fact]
            public async Task WhenToAddressIsInContractRepo_ReturnsTrue()
            {
                _transaction.To = "0x1009b29f2094457d3dba62d1953efea58176ba27";
                InitProcessor();
                _contractHandler.Setup(r => r.ExistsAsync(_transaction.To)).ReturnsAsync(true);

                Assert.True(await _processor.IsTransactionForContractAsync(_transaction));
            }
        }
    }
