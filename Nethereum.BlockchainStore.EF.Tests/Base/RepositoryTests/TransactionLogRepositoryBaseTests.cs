using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nethereum.BlockchainStore.EF.Repositories;
using Newtonsoft.Json.Linq;

namespace Nethereum.BlockchainStore.EF.Tests.Base.RepositoryTests
{
    public abstract class TransactionLogRepositoryBaseTests: RepositoryTestBase
    {
        protected TransactionLogRepositoryBaseTests(IBlockchainDbContextFactory contextFactory) : base(contextFactory)
        {
        }

        [TestMethod]
        public async Task UpsertAsync()
        {
            var transactionLogRepository = new TransactionLogRepository(contextFactory);

            var transactionHash = "0x020af76554bd67c6c716a70bf214eaf7284a483dd8597d7761f78fce11c83a0a";

            var address = "0xba0ef20713557e1c28e12464e4310dff04c0b3ba";
            var data = "0x00000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000015426567696e20726567697374657256656869636c650000000000000000000000";
            long logIndex = 0;
            var topic0 = "0x16da62289fb03291b4545b27ad6fd3329305dfae113f331e8523fcb90148cc65";
            var topic1 = "0x0000000000000000000000000000000000000000000000000000000000000001";
            var topic2 = "0x000000000000000000000000320bd41cba845a47ef34d8218cec1c01728c42ff";
            var topic3 = "0x0000000000000000000000005bab3bf57ff717ca6f21d037c6520ab84634897c";

            var log = JObject.Parse(
                $@"{{
                    address: '{address}', 
                    data: '{data}', 
                    topics: 
                    [
                        '{topic0}',
                        '{topic1}',
                        '{topic2}',
                        '{topic3}'
                    ]
                }}");

            await transactionLogRepository.UpsertAsync(transactionHash, logIndex, log);

            var context = contextFactory.CreateContext();
            var storedLog = await context.TransactionLogs.FindByTransactionHashAndLogIndex(transactionHash, logIndex);

            Assert.IsNotNull(storedLog);

            Assert.AreEqual(transactionHash, storedLog.TransactionHash);
            Assert.AreEqual(logIndex, storedLog.LogIndex);
            Assert.AreEqual(address, storedLog.Address);
            Assert.AreEqual(data, storedLog.Data);
            Assert.AreEqual(topic0, storedLog.EventHash);
            Assert.AreEqual(topic1, storedLog.IndexVal1);
            Assert.AreEqual(topic2, storedLog.IndexVal2);
            Assert.AreEqual(topic3, storedLog.IndexVal3);


        }
    }
}
