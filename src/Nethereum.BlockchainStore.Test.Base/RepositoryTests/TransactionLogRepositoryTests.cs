using Nethereum.BlockchainProcessing.BlockStorage.Repositories;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainStore.Test.Base.RepositoryTests
{
    public class TransactionLogRepositoryTests: IRepositoryTest
    {
        private readonly ITransactionLogRepository _repo;

        public TransactionLogRepositoryTests(ITransactionLogRepository repo)
        {
            _repo = repo;
        }

        public async Task RunAsync()
        {
            await UpsertAsync();
        }

        public async Task UpsertAsync()
        {
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
                    transactionHash: '{transactionHash}',
                    address: '{address}', 
                    data: '{data}', 
                    logIndex: '{logIndex}',
                    topics: 
                    [
                        '{topic0}',
                        '{topic1}',
                        '{topic2}',
                        '{topic3}'
                    ]
                }}").ToObject<FilterLog>();

            var filterLogVO = new FilterLogVO(transaction: null, receipt: null, log);

            await _repo.UpsertAsync(filterLogVO);

            var storedLog = await _repo.FindByTransactionHashAndLogIndexAsync(transactionHash, logIndex);

            Assert.NotNull(storedLog);

            Assert.Equal(transactionHash, storedLog.TransactionHash);
            Assert.Equal(logIndex.ToString(), storedLog.LogIndex);
            Assert.Equal(address, storedLog.Address);
            Assert.Equal(data, storedLog.Data);
            Assert.Equal(topic0, storedLog.EventHash);
            Assert.Equal(topic1, storedLog.IndexVal1);
            Assert.Equal(topic2, storedLog.IndexVal2);
            Assert.Equal(topic3, storedLog.IndexVal3);


        }
    }
}
