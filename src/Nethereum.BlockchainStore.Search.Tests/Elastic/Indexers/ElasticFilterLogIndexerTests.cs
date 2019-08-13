using Nethereum.BlockchainStore.Search.ElasticSearch;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainStore.Search.Tests.Elastic
{
    public class ElasticFilterLogIndexerTests
    {
        [Fact]
        public async Task MapsFilterLogToGenericSearchDocument()
        {
            var mockElasticClient = new MockElasticClient();

            var indexer = new ElasticFilterLogIndexer("my-index", mockElasticClient.ElasticClient);

            var filterLog = new FilterLog
            {
                BlockNumber = new Hex.HexTypes.HexBigInteger(4309),
                TransactionHash = "0x3C81039C578811A85742F2476DA90E363A88CA93763DB4A194E35367D9A72FD8",
                LogIndex = new Hex.HexTypes.HexBigInteger(9)
            };

            await indexer.IndexAsync(filterLog);

            var bulkRequest = mockElasticClient.BulkRequests.FirstOrDefault();

            Assert.Single(bulkRequest.Operations);
            var searchDocument = bulkRequest.Operations.First().GetBody() as GenericSearchDocument;
            Assert.Equal(filterLog.Key(), searchDocument[PresetSearchFieldName.log_key.ToString()]);
        }
    }
}
