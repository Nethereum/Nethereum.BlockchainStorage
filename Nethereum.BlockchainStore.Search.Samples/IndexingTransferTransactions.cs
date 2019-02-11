using System;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.BlockchainProxy;
using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainStore.Search.Azure;
using Nethereum.Configuration;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using Xunit;

namespace Nethereum.BlockchainStore.Search.Samples
{
    public class IndexingFunctionMessages
    {
        [Function("transfer", "bool")]
        public class TransferFunction: FunctionMessage
        {
            [Parameter("address", "_to", 1)]
            public string To {get; set;}
            [Parameter("uint256", "_value", 2)]
            public BigInteger Value {get; set;}
        }

        public const string ApiKeyName = "AzureSearchApiKey";
        public const string AzureSearchServiceName = "blockchainsearch";
        public const string AzureTransferIndexName = "transferfunction";

        private readonly string _azureSearchApiKey;

        public IndexingFunctionMessages()
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

        [Fact]
        public async Task IndexingTransferFunctions()
        {
            using (var azureSearchService = new AzureEventSearchService(AzureSearchServiceName, _azureSearchApiKey))
            {
                await azureSearchService.DeleteIndexAsync(AzureTransferIndexName);

                using (var azureFunctionMessageIndexer =
                    await azureSearchService.GetOrCreateFunctionIndex<TransferFunction>(indexName: AzureTransferIndexName))
                {
                    var transferHandler =
                        new FunctionIndexTransactionHandler<TransferFunction>(azureFunctionMessageIndexer);

                    var web3 = new Web3.Web3("https://rinkeby.infura.io/v3/25e7b6dfc51040b3bfc0e47317d38f60");
                    var blockchainProxy = new BlockchainProxyService(web3);
                    var handlers = new HandlerContainer {TransactionHandler = transferHandler};
                    var blockProcessor = BlockProcessorFactory.Create(blockchainProxy, handlers);
                    var processingStrategy = new ProcessingStrategy(blockProcessor);
                    var blockchainProcessor = new BlockchainProcessor(processingStrategy);

                    var blockRange = new BlockRange(3146684, 3146694);
                    await blockchainProcessor.ProcessAsync(blockRange);

                    await Task.Delay(TimeSpan.FromSeconds(5));

                    //ensure we have written the expected docs to the index
                    Assert.Equal(3, await azureFunctionMessageIndexer.DocumentCountAsync());
                }

                await azureSearchService.DeleteIndexAsync(AzureTransferIndexName);
            }


        }
    }
}
