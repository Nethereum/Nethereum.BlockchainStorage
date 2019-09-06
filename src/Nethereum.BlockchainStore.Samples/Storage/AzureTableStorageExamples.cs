using Nethereum.BlockchainStore.AzureTables.Bootstrap;
using Nethereum.Hex.HexTypes;
using Nethereum.Microsoft.Configuration.Utils;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainStore.Samples.Storage
{
    public class AzureTableStorageExamples
    {

        private readonly Web3.Web3 _web3;
        private readonly string _azureConnectionString;
        private const string URL = "https://rinkeby.infura.io/v3/7238211010344719ad14a89db874158c";

        public AzureTableStorageExamples()
        {
            _web3 = new Web3.Web3(URL);
            ConfigurationUtils.SetEnvironmentAsDevelopment();
            var config = ConfigurationUtils.Build(args: Array.Empty<string>(), userSecretsId: "Nethereum.BlockchainStore.AzureTables");
            _azureConnectionString = config["AzureStorageConnectionString"];
        }

        [Fact]
        public async Task BlockStorageProcessing()
        {
            var repoFactory = new AzureTablesRepositoryFactory(_azureConnectionString, "bspsamples");

            try
            { 
                //create our processor
                var processor = _web3.Processing.Blocks.CreateBlockStorageProcessor(repoFactory);

                //if we need to stop the processor mid execution - call cancel on the token
                var cancellationToken = new CancellationToken();

                //crawl the required block range
                await processor.ExecuteAsync(
                    toBlockNumber: new BigInteger(2830144),
                    cancellationToken: cancellationToken,
                    startAtBlockNumberIfNotProcessed: new BigInteger(2830144));

                var block = await repoFactory.CreateBlockRepository().FindByBlockNumberAsync(new HexBigInteger(2830144));
                Assert.NotNull(block);
            }
            finally 
            { 
                await repoFactory.DeleteAllTables();
            }
        }
    }
}
