using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Search.Models;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.BlockchainStore.Search.Azure;
using Nethereum.Configuration;
using Nethereum.Contracts;
using Xunit;

namespace Nethereum.BlockchainStore.Search.Samples.Azure
{
    [Collection("Nethereum.BlockchainStore.Search.Samples.Azure")]
    public class IndexingEventsAndRelatedFunctions
    {
        public const string ApiKeyName = "AzureSearchApiKey";
        public const string AzureSearchServiceName = "blockchainsearch";
        private const string BlockchainUrl = TestConfiguration.BlockchainUrls.Infura.Rinkeby;
        private readonly string _azureSearchApiKey;

        public const string TransferEventIndexName = "transfer-event";
        public const string TransferFunctionIndexName = "transfer-function";
        public const string TransferFromFunctionIndexName = "transfer-from-function";

        public IndexingEventsAndRelatedFunctions()
        {
            //user secrets are only for development
            //if not in development the key will be retrieved from environmental variables or command line args
            ConfigurationUtils.SetEnvironment("development");

            //use the command line to set your azure search api key
            //e.g. dotnet user-secrets set "AzureSearchApiKey" "<put key here>"
            var appConfig = ConfigurationUtils
                .Build(Array.Empty<string>(), userSecretsId: "Nethereum.BlockchainStore.Search.Samples");

            _azureSearchApiKey = appConfig[ApiKeyName];
        }

        [Function("transfer", "bool")]
        public class TransferFunction: FunctionMessage
        {
            [Parameter("address", "_to", 1)]
            public string To {get; set;}
            [Parameter("uint256", "_value", 2)]
            public BigInteger Value {get; set;}
        }

        [Function("transferFrom", "bool")]
        public class TransferFromFunction: FunctionMessage
        {
            [Parameter("address", "_from", 1)]
            public string From {get; set;}
            [Parameter("address", "_to", 2)]
            public string To {get; set;}
            [Parameter("uint256", "_value", 3)]
            public BigInteger Value {get; set;}
        }

        [Event("Transfer")]
        public class TransferEvent_ERC20
        {
            [Parameter("address", "_from", 1, true)]
            public string From {get; set;}

            [Parameter("address", "_to", 2, true)]
            public string To {get; set;}

            [Parameter("uint256", "_value", 3, false)]
            public BigInteger Value {get; set;}
        }

        // function approve(address spender, uint256 value) external returns (bool);
        // event Approval(address indexed owner, address indexed spender, uint256 value);

        /// <summary>
        /// Indexing Transfer events and the input parameters from calls to the 'transfer' or 'transferFrom' function that instigated the event
        /// </summary>
        [Fact]
        public async Task IndexTransferEventAndFunction()
        {
            using (var processor =
                new AzureEventIndexingProcessor(AzureSearchServiceName, _azureSearchApiKey, BlockchainUrl))
            {
                await ClearDown(processor);
                try
                {
                    //reusable function handler which incorporates a function indexer
                    //the function indexer won't do anything yet
                    //it must be linked to an event processor before functions are indexed
                    var transferFunctionHandler =
                        await processor.CreateFunctionHandlerAsync<TransferFunction>(TransferFunctionIndexName);

                    //reusable function handler which incorporates a function indexer
                    //the function indexer won't do anything yet
                    //it must be linked to an event processor before functions are indexed
                    var transferFromFunctionHandler =
                        await processor.CreateFunctionHandlerAsync<TransferFromFunction>(TransferFromFunctionIndexName);

                    //create an indexer for the transfer event
                    //link our function handlers so that functions related to the event are indexed
                    await processor.AddAsync<TransferEvent_ERC20>(TransferEventIndexName, new ITransactionHandler[]
                    {
                        transferFunctionHandler, transferFromFunctionHandler
                    });

                    //process the range
                    await processor.ProcessAsync(3900007, 3900030);

                    //allow time for azure indexing to finish
                    await Task.Delay(TimeSpan.FromSeconds(5));

                    //ensure we have written to the expected indexes
                    long transferEventCount = await processor.SearchService.CountDocumentsAsync(TransferEventIndexName);
                    long transferFunctionCount =
                        await processor.SearchService.CountDocumentsAsync(TransferFunctionIndexName);
                    long transferFromFunctionCount =
                        await processor.SearchService.CountDocumentsAsync(TransferFromFunctionIndexName);

                    Assert.Equal(32, transferEventCount);
                    Assert.Equal(2, transferFunctionCount);
                    Assert.Equal(2, transferFromFunctionCount);
                }
                finally
                {
                    await ClearDown(processor);
                }
            }
        }

        private static async Task ClearDown(AzureEventIndexingProcessor processor)
        {
            #region test preparation
            await processor.ClearProgress();
            await processor.SearchService.DeleteIndexAsync(TransferEventIndexName);
            await processor.SearchService.DeleteIndexAsync(TransferFunctionIndexName);
            await processor.SearchService.DeleteIndexAsync(TransferFromFunctionIndexName);
            #endregion
        }

        /// <summary>
        /// a custom dto to define what transfer data is captured in the search index
        /// </summary>
        public class CustomTransferEventSearchDocument
        {
            public string DocumentKey { get; set; }
            public string From { get; set; }
            public string To { get; set; }
            public string Value { get; set; }
        }

        /// <summary>
        /// a custom dto to define what transfer function data is captured in the search index
        /// </summary>
        public class CustomTransferFunctionSearchDocument
        {
            public string TransactionHash { get; set; }
            public string BlockNumber { get; set; }
            public string BlockTimestamp { get; set; }
            public string To { get; set; }
            public string Value { get; set; }
        }

        [Fact]
        public async Task StoringCustomSearchDocuments()
        {
            using (var processor =
                new AzureEventIndexingProcessor(AzureSearchServiceName, _azureSearchApiKey, BlockchainUrl))
            {
                await ClearDown(processor);

                try
                {
                    //define our azure index for transfer function data
                    var transferFunctionIndex = new Index
                    {
                        Name = TransferFunctionIndexName,
                        Fields =
                            new List<Field>
                            {
                                new Field("TransactionHash", DataType.String) {IsKey = true, IsSearchable = true},
                                new Field("BlockNumber", DataType.String) {IsSearchable = true},
                                new Field("BlockTimestamp", DataType.String),
                                new Field("To", DataType.String),
                                new Field("Value", DataType.String)
                            }
                    };

                    //define azure index for transfer event data
                    var transferEventIndex = new Index
                    {
                        Name = TransferEventIndexName,
                        Fields = new List<Field>
                        {
                            new Field("DocumentKey", DataType.String) {IsKey = true},
                            new Field("From", DataType.String) {IsSearchable = true, IsFacetable = true},
                            new Field("To", DataType.String) {IsSearchable = true, IsFacetable = true},
                            new Field("Value", DataType.String)
                        }
                    };

                    //reusable function handler which incorporates a function indexer
                    //the function indexer won't do anything yet
                    //it must be linked to an event processor before functions are indexed
                    var transferFunctionHandler =
                        await processor
                            .CreateFunctionHandlerAsync<TransferFunction, CustomTransferFunctionSearchDocument>(
                                transferFunctionIndex, (tx) => new CustomTransferFunctionSearchDocument
                                {
                                    TransactionHash = tx.Tx.TransactionHash,
                                    BlockNumber = tx.Tx.BlockNumber.Value.ToString(),
                                    BlockTimestamp = tx.Tx.BlockTimestamp.ToString(),
                                    To = tx.Dto.To,
                                    Value = tx.Dto.Value.ToString()
                                });


                    //create an indexer for the transfer event
                    //link our function handlers so that functions related to the event are indexed
                    await processor.AddAsync<TransferEvent_ERC20, CustomTransferEventSearchDocument>(
                        transferEventIndex, (eventLog) => new CustomTransferEventSearchDocument
                        {
                            From = eventLog.Event.From,
                            To = eventLog.Event.To,
                            Value = eventLog.Event.Value.ToString(),
                            DocumentKey = $"{eventLog.Log.TransactionHash}_{eventLog.Log.LogIndex.Value}"
                        }
                        , new ITransactionHandler[]
                        {
                            transferFunctionHandler
                        });

                    //process the range
                    await processor.ProcessAsync(3146684, 3146694);

                    //allow time for azure indexing to finish
                    await Task.Delay(TimeSpan.FromSeconds(5));

                    //ensure we have written to the expected indexes
                    long transferEventCount = await processor.SearchService.CountDocumentsAsync(TransferEventIndexName);
                    long transferFunctionCount =
                        await processor.SearchService.CountDocumentsAsync(TransferFunctionIndexName);

                    Assert.Equal(19, transferEventCount);
                    Assert.Equal(1, transferFunctionCount);
                }
                finally
                {
                    await ClearDown(processor);
                }
            }
        }
    }
}
