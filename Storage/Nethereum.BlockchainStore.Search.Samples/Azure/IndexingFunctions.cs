using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainStore.Search.Azure;
using Nethereum.Configuration;
using Nethereum.Contracts;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainStore.Search.Samples.Azure
{
    [Collection("Nethereum.BlockchainStore.Search.Samples.Azure")]
    public class IndexingFunctions
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

        public IndexingFunctions()
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
        public async Task IndexingTransferFunctions()
        {
            using (var azureSearchService = new AzureSearchService(AzureSearchServiceName, _azureSearchApiKey))
            {
                await azureSearchService.DeleteIndexAsync(AzureTransferIndexName);

                try
                {
                    using (var azureFunctionMessageIndexer =
                        await azureSearchService.CreateFunctionIndexer<TransferFunction>(
                            indexName: AzureTransferIndexName))
                    {
                        var transferHandler =
                            new FunctionIndexTransactionHandler<TransferFunction>(azureFunctionMessageIndexer);

                        var web3 = new Web3.Web3(TestConfiguration.BlockchainUrls.Infura.Rinkeby);
                        var handlers = new HandlerContainer {TransactionHandler = transferHandler};
                        var blockProcessor = BlockProcessorFactory.Create(web3, handlers);
                        var processingStrategy = new ProcessingStrategy(blockProcessor);
                        var blockchainProcessor = new BlockchainProcessor(processingStrategy);

                        var blockRange = new BlockRange(3146684, 3146694);
                        await blockchainProcessor.ProcessAsync(blockRange);

                        await Task.Delay(TimeSpan.FromSeconds(5));

                        //ensure we have written the expected docs to the index
                        Assert.Equal(3, await azureFunctionMessageIndexer.DocumentCountAsync());
                    }
                }
                finally
                {
                    await azureSearchService.DeleteIndexAsync(AzureTransferIndexName);
                }
            }


        }
    }
}
