using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainStore.Search.Azure;
using Nethereum.Contracts;
using Nethereum.Microsoft.Configuration.Utils;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Util;
using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainStore.Search.Samples.Azure
{
    [Collection("Nethereum.BlockchainStore.Search.Samples.Azure")]
    public class IndexingFunctions
    {
        private const string BlockchainUrl = TestConfiguration.BlockchainUrls.Infura.Rinkeby;

        [Function("transfer", "bool")]
        public class TransferFunction : FunctionMessage
        {
            [Parameter("address", "_to", 1)]
            public string To { get; set; }
            [Parameter("uint256", "_value", 2)]
            public BigInteger Value { get; set; }
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
            const string INDEX_NAME = "transfer-functions";

            //surround with "using" so that anything in a buffer is sent on dispose
            using (var azureSearchService = new AzureSearchService(AzureSearchServiceName, _azureSearchApiKey))
            {
                try
                {
                    //setup
                    var index = await azureSearchService.CreateIndexForFunctionAsync<TransferFunction>(INDEX_NAME);

                    var searchIndexProcessor = azureSearchService.CreateFunctionMessageProcessor<TransferFunction>(index, documentsPerBatch: 1);
                    var web3 = new Web3.Web3(BlockchainUrl);

                    var blockchainProcessor = web3.Processing.Blocks.CreateBlockProcessor((steps) => {
                        steps.TransactionStep.SetMatchCriteria(tx => tx.Transaction.IsFrom("0xf47a8bb5c9ff814d39509591281ae31c0c7c2f38"));
                        steps.TransactionReceiptStep.AddProcessorHandler(searchIndexProcessor);
                    });

                    var cancellationTokenSource = new CancellationTokenSource();

                    //execute
                    await blockchainProcessor.ExecuteAsync(3146689, cancellationTokenSource.Token, 3146689);

                    //assert
                    await Task.Delay(5000); // allow time to index
                    Assert.Equal(1, await azureSearchService.CountDocumentsAsync(INDEX_NAME));

                }
                finally
                {
                    await azureSearchService.DeleteIndexAsync(INDEX_NAME);
                }
            }

        }
    }
}
