using System.Threading.Tasks;
using Nethereum.BlockchainStore.EFCore;
using Nethereum.BlockchainStore.EFCore.Repositories;
using Xunit;

namespace Nethereum.BlockchainStore.Tests.EFCore.Base.RepositoryTests
{
    public abstract class ContractRepositoryBaseTests: RepositoryTestBase
    {
        protected ContractRepositoryBaseTests(IBlockchainDbContextFactory contextFactory) : base(contextFactory)
        {
        }

        [Fact]
        public async Task UpsertAsync()
        {
            var contractRepository = new ContractRepository(contextFactory);

            var context = contextFactory.CreateContext();

            var contractAddress = "0x26bc47888b7bfdf77db41ec0a2fb4db00af1c92a";
            var code = "0x6080604052600436106053576000357c0100000000000000000000000000000000000000000000000000000000900463ffffffff1680635589f21d14605857806388a6f572146076578063ddca3f4314609e575b600080fd5b60746004803603810190808035906020019092919050505060c6565b005b348015608157600080fd5b50608860d0565b6040518082815260200191505060405180910390f35b34801560a957600080fd5b5060b060d9565b6040518082815260200191505060405180910390f35b8060008190555050565b60008054905090565b600054815600a165627a7a723058205345477b840b4fb7b6401abdcd3ab98ae1db0d342e6c6b4b45a4e2b10f6ae1f80029";

            var transaction = new Nethereum.RPC.Eth.DTOs.Transaction
            {
                From = "0xe6de16a66e5cd7270cc36a851818bc092884fe64",
                TransactionHash = "0xcb00b69d2594a3583309f332ada97d0df48bae00170e36a4f7bbdad7783fc7e5"
            };

            var existingContract = await context.Contracts.FindByContractAddressAsync(contractAddress);
            if (existingContract != null)
            {
                context.Contracts.Remove(existingContract);
                await context.SaveChangesAsync();
            }

            Assert.False(await contractRepository.ExistsAsync(contractAddress));
            Assert.False(contractRepository.IsCached(contractAddress));

            await contractRepository.UpsertAsync(contractAddress, code, transaction);

            Assert.True(await contractRepository.ExistsAsync(contractAddress));
            Assert.True(contractRepository.IsCached(contractAddress));

            
            var storedContract = await context.Contracts.FindByContractAddressAsync(contractAddress);

            Assert.NotNull(storedContract);
            Assert.Equal(contractAddress, storedContract.Address);
            Assert.Equal(code, storedContract.Code);
            Assert.Equal(transaction.From, storedContract.Creator);
            Assert.Equal(transaction.TransactionHash, storedContract.TransactionHash);

        }
    }
}
