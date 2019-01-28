using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.BlockchainProxy;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainStore.Search.Azure;
using System;
using System.IO;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Search.POC
{
    public class TransferIndexingService: IDisposable
    {

        /*
Solidity Contract Excerpt
* event Transfer(address indexed _from, address indexed _to, uint256 _value);
*/
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

        /*
Solidity Contract Excerpt
* event Transfer(address indexed _from, address indexed _to, uint256 indexed _value);
*/
        [Event("Transfer")]
        public class TransferEvent_Custom
        {
            [Parameter("address", "_from", 1, true)]
            public string From {get; set;}

            [Parameter("address", "_to", 2, true)]
            public string To {get; set;}

            [Parameter("uint256", "_value", 3, true)]
            public BigInteger Value {get; set;}
        }

        private readonly string transferIndexName;
        private readonly DisposableContext d;
        private readonly BlockchainProxyService blockchainProxyService;
        private readonly AzureEventSearchService searchService;
        private readonly JsonBlockProgressRepository progressRepository;
        private readonly string _azureSearchApiKey;

        private const int LogsPerIndexBatch = 10;

        public TransferIndexingService(string blockchainUrl, string azureSearchServiceName, string azureSearchApiKey, string indexName)
        {
            d = new DisposableContext();
            blockchainProxyService = new BlockchainProxyService(blockchainUrl);
            searchService = d.Add(() => new AzureEventSearchService(azureSearchServiceName, azureSearchApiKey));

            progressRepository =
                new JsonBlockProgressRepository(CreateJsonFileToHoldProgress());

            transferIndexName = indexName;
        }

        public async Task RunContinually(ulong fromBlockNumber, CancellationToken cancellationToken)
        {
            var customIndexer = await d.Add(() => searchService.GetOrCreateIndex<TransferEvent_Custom>(transferIndexName));
            var customProcessor = d.Add(() => new EventLogSearchIndexProcessor<TransferEvent_Custom>(customIndexer, logsPerIndexBatch: LogsPerIndexBatch));

            //add the erc 20 event -
            //this will use the same azure index as the custom event - it won't be re-created or altered
            //the "value" element will still be indexed in azure
            var erc20Indexer = await d.Add(() => searchService.GetOrCreateIndex<TransferEvent_ERC20>(transferIndexName));
            var erc20Processor = d.Add(() => new EventLogSearchIndexProcessor<TransferEvent_ERC20>(erc20Indexer, logsPerIndexBatch: LogsPerIndexBatch));

            var logProcessor = new BlockchainLogProcessor(
                blockchainProxyService,
                new ILogProcessor[] {erc20Processor, customProcessor});

            //this will get the last block on the chain each time a "to" block is requested
            var progressService = new BlockProgressService(
                blockchainProxyService, fromBlockNumber, progressRepository)
            {
                MinimumBlockConfirmations = 6 //stay within x blocks of the most recent
            };

            var batchProcessorService = new BlockchainBatchProcessorService(
                logProcessor, progressService, maxNumberOfBlocksPerBatch: 10);

            await batchProcessorService.ProcessContinuallyAsync(cancellationToken);

        }

        private static string CreateJsonFileToHoldProgress()
        {
            var progressFileNameAndPath = Path.Combine(Path.GetTempPath(), "TransferIndexerProgress.json");
            if (File.Exists(progressFileNameAndPath)) File.Delete(progressFileNameAndPath);
            return progressFileNameAndPath;
        }

        public void Dispose()
        {
            d.Dispose();
        }
    }
}
