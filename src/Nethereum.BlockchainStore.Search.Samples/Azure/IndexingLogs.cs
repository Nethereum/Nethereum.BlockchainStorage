using Microsoft.Azure.Search.Models;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainStore.Search.Azure;
using Nethereum.Contracts;
using Nethereum.Microsoft.Configuration.Utils;
using Nethereum.Util;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Index = Microsoft.Azure.Search.Models.Index;

namespace Nethereum.BlockchainStore.Search.Samples.Azure
{
    [Collection("Nethereum.BlockchainStore.Search.Samples.Azure")]
    public class IndexingLogs
    {
        /*
Solidity Contract Excerpt
* event Transfer(address indexed _from, address indexed _to, uint256 _value);
*/
        [Event("Transfer")]
        public class TransferEvent_ERC20 : IEventDTO
        {
            [Parameter("address", "_from", 1, true)]
            public string From {get; set;}

            [Parameter("address", "_to", 2, true)]
            public string To {get; set;}

            [Parameter("uint256", "_value", 3, false)]
            public BigInteger Value {get; set;}
        }

        public const string ApiKeyName = "AzureSearchApiKey";
        public const string AzureSearchServiceName = "blockchainsearch";
        public const string AzureTransferIndexName = "transfer";
        private const string BlockchainUrl = TestConfiguration.BlockchainUrls.Infura.Rinkeby;
        private readonly string _azureSearchApiKey;

        public IndexingLogs()
        {
            //user secrets are only for development
            //if not in development the key will be retrieved from environmental variables or command line args
            ConfigurationUtils.SetEnvironmentAsDevelopment();

            //use the command line to set your azure search api key
            //e.g. dotnet user-secrets set "AzureSearchApiKey" "<put key here>"
            var appConfig = ConfigurationUtils
                .Build(Array.Empty<string>(), userSecretsId: "Nethereum.BlockchainStore.Search.Samples");

            _azureSearchApiKey = appConfig[ApiKeyName];
        }

        [Fact]
        public async Task OneEvent()
        {
            const string INDEX_NAME = "transfer-logs";

            //surround with "using" so that anything in a buffer is sent on dispose
            using (var azureSearchService = new AzureSearchService(AzureSearchServiceName, _azureSearchApiKey))
            { 
                try
                {
                    //setup
                    var index = await azureSearchService.CreateIndexForEventLogAsync<TransferEvent_ERC20>(INDEX_NAME);

                    var indexer = azureSearchService.CreateIndexerForEventLog<TransferEvent_ERC20>(index.Name, documentsPerBatch: 1);

                    var web3 = new Web3.Web3(BlockchainUrl);
                    var blockchainProcessor = web3.Processing.Logs.CreateProcessor<TransferEvent_ERC20>((transfer) => indexer.IndexAsync(transfer));
                    var cancellationTokenSource = new CancellationTokenSource();

                    //execute
                    await blockchainProcessor.ExecuteAsync(3146694, cancellationTokenSource.Token, 3146684);

                    //assert
                    await Task.Delay(5000); // allow time to index
                    Assert.Equal(19, await azureSearchService.CountDocumentsAsync(INDEX_NAME));
                    
                }
                finally
                {
                    await azureSearchService.DeleteIndexAsync(INDEX_NAME);
                }
            }
        }

        [Fact]
        public async Task OneEventWithMapping()
        {
            const string INDEX_NAME = "one-event-with-mapping";

            using (var azureSearchService = new AzureSearchService(AzureSearchServiceName, _azureSearchApiKey))
            {
                try
                {
                    //setup

                    //create a definition of an index - 
                    //it describes the fields, key and data types
                    //at this stage - it is only an in-memory schema
                    var index = CreateAzureIndexDefinition(INDEX_NAME);

                    //create the index in azure
                    index = await azureSearchService.CreateIndexAsync(index);

                    // create a processor for a specific event and map to a custom DTO for the search
                    var indexer = azureSearchService.CreateIndexerForEventLog<TransferEvent_ERC20, CustomTransferSearchDocumentDto>(
                        index.Name,
                        (e) => 
                            new CustomTransferSearchDocumentDto
                            {
                                From = e.Event.From,
                                To = e.Event.To,
                                Value = e.Event.Value.ToString(),
                                BlockNumber = e.Log.BlockNumber.Value.ToString(),
                                TxHash = e.Log.TransactionHash,
                                LogAddress = e.Log.Address,
                                LogIndex = (int)e.Log.LogIndex.Value,
                                DocumentKey = $"{e.Log.TransactionHash}_{e.Log.LogIndex.Value}"
                            },
                        documentsPerBatch: 1);

                    var web3 = new Web3.Web3(BlockchainUrl);
                    var blockchainProcessor = web3.Processing.Logs.CreateProcessor<TransferEvent_ERC20>(
                        transfer => indexer.IndexAsync(transfer));

                    var cancellationTokenSource = new CancellationTokenSource();

                    //execute
                    await blockchainProcessor.ExecuteAsync(3146694, cancellationTokenSource.Token, 3146684);

                    //assert
                    await Task.Delay(5000); // allow time to index
                    Assert.Equal(19, await azureSearchService.CountDocumentsAsync(INDEX_NAME));

                }
                finally
                {
                    await azureSearchService.DeleteIndexAsync(INDEX_NAME);
                }
            }
        }

        [Fact]
        public async Task FilterLogs()
        {
            const string INDEX_NAME = "filter-logs";

            //surround with "using" so that anything in a buffer is sent on dispose
            //to clear the buffer manually - searchIndexProcessor.EventIndexer.SubmitPendingItemsAsync()
            using (var azureSearchService = new AzureSearchService(AzureSearchServiceName, _azureSearchApiKey))
            { 
                try
                {
                    // create an index - if an existing index is required: azureSearchService.GetIndexAsync()
                    var index = await azureSearchService.CreateIndexForLogAsync(INDEX_NAME);

                    var indexer = azureSearchService.CreateIndexerForLog(index.Name, documentsPerBatch: 1);
                    var web3 = new Web3.Web3(BlockchainUrl);
                    var blockchainProcessor = web3.Processing.Logs.CreateProcessor(log => indexer.IndexAsync(log));
                    var cancellationTokenSource = new CancellationTokenSource();
                    await blockchainProcessor.ExecuteAsync(3146685, cancellationTokenSource.Token, 3146684);
                        
                    await Task.Delay(5000); // allow time to index
                    Assert.Equal(25, await azureSearchService.CountDocumentsAsync(INDEX_NAME));
                }
                finally
                {
                    await azureSearchService.DeleteIndexAsync(INDEX_NAME);
                }
            }
        }

        [Fact]
        public async Task FilterLogsWithCriteria()
        {
            const string INDEX_NAME = "filter-logs-with-criteria";

            //surround with "using" so that anything in a buffer is sent on dispose
            using (var azureSearchService = new AzureSearchService(AzureSearchServiceName, _azureSearchApiKey))
            {
                try
                {
                    // create an index - if an existing index is required: azureSearchService.GetIndexAsync()
                    var index = await azureSearchService.CreateIndexForLogAsync(INDEX_NAME);

                    var indexer = azureSearchService.CreateIndexerForLog(
                        index.Name,
                        documentsPerBatch: 1);

                    var web3 = new Web3.Web3(BlockchainUrl);

                    var blockchainProcessor = web3.Processing.Logs.CreateProcessor(
                        action: log => indexer.IndexAsync(log), 
                        criteria: log => AddressUtil.Current.AreAddressesTheSame(log.Address, "0x9edcb9a9c4d34b5d6a082c86cb4f117a1394f831"));

                    var cancellationTokenSource = new CancellationTokenSource();
                    await blockchainProcessor.ExecuteAsync(3146685, cancellationTokenSource.Token, 3146684);

                    await Task.Delay(5000); // allow time to index
                    Assert.Equal(2, await azureSearchService.CountDocumentsAsync(INDEX_NAME));
                }
                finally
                {
                    await azureSearchService.DeleteIndexAsync(INDEX_NAME);
                }
            }
        }

        [Function("transfer", "bool")]
        public class TransferFunction : FunctionMessage
        {
            [Parameter("address", "_to", 1)]
            public string To { get; set; }
            [Parameter("uint256", "_value", 2)]
            public BigInteger Value { get; set; }
        }

        [Fact]
        public async Task AnEventAndItsTransaction()
        {
            const string EVENT_INDEX_NAME = "transfer-logs-related";
            const string FUNCTION_INDEX_NAME = "related-transfer-functions";

            //surround with "using" so that anything in a buffer is sent on dispose
            using (var azureSearchService = new AzureSearchService(AzureSearchServiceName, _azureSearchApiKey))
            {
                try
                {
                    //setup
                    var transferEventIndex = await azureSearchService.CreateIndexForEventLogAsync<TransferEvent_ERC20>(EVENT_INDEX_NAME);
                    var transferFunctionIndex = await azureSearchService.CreateIndexForFunctionMessageAsync<TransferFunction>(FUNCTION_INDEX_NAME);

                    var transferIndexer = azureSearchService.CreateIndexerForEventLog<TransferEvent_ERC20>(transferEventIndex.Name);
                    var transferFunctionIndexer = azureSearchService.CreateIndexerForFunctionMessage<TransferFunction>(transferFunctionIndex.Name);

                    //this handler ensures the transaction is a Transfer and invokes the indexer
                    var transferFunctionProcessorHandler = transferFunctionIndexer.CreateProcessorHandler();

                    var web3 = new Web3.Web3(BlockchainUrl);

                    var blockchainProcessor = web3.Processing.Logs.CreateProcessor<TransferEvent_ERC20>(async (log) => {

                        await transferIndexer.IndexAsync(log);
                        var transactionWithReceipt = await web3.Eth.GetTransactionReceiptVO(log.Log.BlockNumber, log.Log.TransactionHash).ConfigureAwait(false);

                        await transferFunctionProcessorHandler.ExecuteAsync(transactionWithReceipt);
                    });

                    var cancellationTokenSource = new CancellationTokenSource();

                    //execute
                    await blockchainProcessor.ExecuteAsync(3146694, cancellationTokenSource.Token, 3146684);

                    //assert
                    await Task.Delay(5000); // allow time to index
                    Assert.Equal(19, await azureSearchService.CountDocumentsAsync(EVENT_INDEX_NAME));
                    Assert.Equal(3, await azureSearchService.CountDocumentsAsync(FUNCTION_INDEX_NAME));
                }
                finally
                {
                    await azureSearchService.DeleteIndexAsync(EVENT_INDEX_NAME);
                    await azureSearchService.DeleteIndexAsync(FUNCTION_INDEX_NAME);
                }
            }
        }

        [Fact]
        public async Task PendingItemsBuffer()
        {
            const string INDEX_NAME = "filter-logs-clearing-buffer";

            using (var azureSearchService = new AzureSearchService(AzureSearchServiceName, _azureSearchApiKey))
            {
                try
                {
                    // create an index - if an existing index is required: azureSearchService.GetIndexAsync()
                    var index = await azureSearchService.CreateIndexForLogAsync(INDEX_NAME);

                    var indexer = azureSearchService.CreateIndexerForLog(INDEX_NAME, documentsPerBatch: 10);
 
                    var web3 = new Web3.Web3(BlockchainUrl);
                    var blockchainProcessor = web3.Processing.Logs.CreateProcessor(log => indexer.IndexAsync(log));
                    var cancellationTokenSource = new CancellationTokenSource();

                    //execute
                    await blockchainProcessor.ExecuteAsync(3146685, cancellationTokenSource.Token, 3146684);

                    //as the indexer processes in batches and we've dictated a size of 10 items per batch
                    //we should have a buffer of items pending submission
                    //these are processed on disposal - but we can force this process manually
                    Assert.Equal(5, indexer.PendingDocumentCount);
                    Assert.Equal(20, indexer.Indexed);
                    //process the pending items
                    await indexer.IndexPendingDocumentsAsync();

                    //the buffer should be clear now
                    Assert.Equal(0, indexer.PendingDocumentCount);
                    Assert.Equal(25, indexer.Indexed);

                    await Task.Delay(5000); // allow time for Azure to index
                    Assert.Equal(25, await azureSearchService.CountDocumentsAsync(INDEX_NAME));

                }
                finally
                {
                    await azureSearchService.DeleteIndexAsync(INDEX_NAME);
                }
            }
        }




        /// <summary>
        /// An example of a custom DTO based on a transfer event to be stored in the azure index
        /// </summary>
        public class CustomTransferSearchDocumentDto: IHasId
        {
            public string From { get; set; }
            public string To { get; set; }
            public string Value { get; set; }
            public string BlockNumber { get; set; }
            public string TxHash { get; set; }
            public string LogAddress { get; set; }
            public int LogIndex { get; set; }

            public string DocumentKey { get; set; }

            public string GetId()
            {
                return DocumentKey;
            }
        }


        private static Index CreateAzureIndexDefinition(string indexName)
        {
            var index = new Index { Name = indexName };
            index.Fields = new List<Field>
                {
                    new Field("DocumentKey", DataType.String) { IsKey = true },
                    new Field("From", DataType.String) { IsSearchable = true, IsFacetable = true },
                    new Field("To", DataType.String) { IsSearchable = true, IsFacetable = true },
                    new Field("Value", DataType.String),
                    new Field("BlockNumber", DataType.String),
                    new Field("TxHash", DataType.String),
                    new Field("LogAddress", DataType.String),
                    new Field("LogIndex", DataType.Int64)
                };
            return index;
        }

    }
}

