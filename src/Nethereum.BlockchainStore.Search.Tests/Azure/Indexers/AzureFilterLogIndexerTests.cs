using Microsoft.Azure.Search.Models;
using Nethereum.BlockchainStore.Search.Azure;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainStore.Search.Tests.Azure
{
    public class AzureFilterLogIndexerTests
    {
        [Fact]
        public async Task MapsFilterLogToGenericSearchDocument()
        {
            var searchIndexClientMock = new SearchIndexClientMock<GenericSearchDocument>();

            var index = FilterLogIndexUtil.Create("my-logs");

            var indexer = new AzureFilterLogIndexer(searchIndexClientMock.SearchIndexClient);

            var filterLog = new FilterLog {
                BlockNumber = new Hex.HexTypes.HexBigInteger(4309), 
                TransactionHash = "0x3C81039C578811A85742F2476DA90E363A88CA93763DB4A194E35367D9A72FD8", 
                LogIndex = new Hex.HexTypes.HexBigInteger(9) };

            await indexer.IndexAsync(filterLog);
            
            var indexedBatch = searchIndexClientMock.IndexedBatches.FirstOrDefault();

            Assert.Single(indexedBatch.Actions);
            var action = indexedBatch.Actions.First();
            Assert.Equal(IndexActionType.MergeOrUpload, action.ActionType);
            Assert.Equal(filterLog.Key(), action.Document[PresetSearchFieldName.log_key.ToString()]);
        }
    }
}
