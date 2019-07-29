using Common.Logging;
using Microsoft.Configuration.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Logging.Utils;
using Nethereum.BlockchainProcessing;
using Nethereum.BlockchainProcessing.BlockProcessing;
using Nethereum.BlockchainProcessing.BlockStorage.Repositories;
using Nethereum.BlockchainProcessing.ProgressRepositories;
using Nethereum.RPC.Eth.Blocks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Console
{

    public class StorageProcessorConsole<TRepositoryFactory> 
        where TRepositoryFactory : IBlockchainStoreRepositoryFactory, IBlockProgressRepositoryFactory
    {
        ILog log = ApplicationLogging.CreateConsoleLogger<StorageProcessorConsole<TRepositoryFactory>>().ToILog();

        public StorageProcessorConsole(
            string[] consoleArgs,
            string userSecretsId,
            Func<IConfigurationRoot, TRepositoryFactory> createRepositoryFactoryCallback,
            Action<TRepositoryFactory> disposeRepositoriesCallback = null
            )
        {
            log.Info("StorageProcessorConsole constructor");
            ConsoleArgs = consoleArgs;
            UserSecretsId = userSecretsId;
            CreateRepositoryFactoryCallback = createRepositoryFactoryCallback;
            DisposeAction = disposeRepositoriesCallback;
            Configuration = ConfigurationUtils.Build(ConsoleArgs, userSecretsId: UserSecretsId);
            TargetConfiguration = BlockchainSourceConfigurationFactory.Get(Configuration);
        }

        public BlockchainSourceConfiguration TargetConfiguration { get;}
        public string[] ConsoleArgs { get; }
        public string UserSecretsId { get; }
        public Func<IConfigurationRoot, TRepositoryFactory> CreateRepositoryFactoryCallback { get; }
        public Action<TRepositoryFactory> DisposeAction { get; }
        public IConfigurationRoot Configuration { get;}
        public async Task<int> ExecuteAsync(
            CancellationToken cancellationToken = default(CancellationToken)) 
        {
            TRepositoryFactory repoFactory = default(TRepositoryFactory);

            try
            {
                repoFactory = CreateRepositoryFactoryCallback(Configuration);

                BlockchainProcessor processor = CreateBlockChainProcessor(repoFactory);

                if (TargetConfiguration.ToBlock != null)
                {
                    await processor.ExecuteAsync(TargetConfiguration.ToBlock.Value, cancellationToken, TargetConfiguration.FromBlock);
                }
                else
                {
                    await processor.ExecuteAsync(cancellationToken, TargetConfiguration.FromBlock);
                }

                return 0;

            }
            catch (Exception ex)
            {
                log?.Error(ex);
                if(repoFactory != null) DisposeAction?.Invoke(repoFactory);
                return 1;
            }

        }

        private BlockchainProcessor CreateBlockChainProcessor(TRepositoryFactory repoFactory)
        {
            var web3 = new Web3.Web3(TargetConfiguration.BlockchainUrl);

            var steps = new BlockProcessingSteps();
            steps.BlockStep.AddProcessorHandler((b) => { log.Info($"Processing block {b.Number.Value}, Tx Count: {b.Transactions.Length}"); return Task.CompletedTask; });
            steps.TransactionReceiptStep.AddProcessorHandler((tx) => { log.Info($"\tTransaction: Index: {tx.Transaction.TransactionIndex}, Hash: {tx.TransactionHash}"); return Task.CompletedTask; });
            steps.FilterLogStep.AddProcessorHandler((l) => { log.Info($"\t\tLog: Index: {l.LogIndex}"); return Task.CompletedTask; });

            var orchestrator = new BlockCrawlOrchestrator(web3.Eth, steps);
            orchestrator.ContractCreatedCrawlerStep.RetrieveCode = true;

            var lastConfirmedBlockNumberService = new LastConfirmedBlockNumberService(
                web3.Eth.Blocks.GetBlockNumber, 
                TargetConfiguration.MinimumBlockConfirmations ?? LastConfirmedBlockNumberService.DEFAULT_BLOCK_CONFIRMATIONS, 
                log);

            IBlockProgressRepository progressRepo = repoFactory.CreateBlockProgressRepository();

            var processor = new BlockchainProcessor(orchestrator, progressRepo, lastConfirmedBlockNumberService, log);

            return processor;
        }
    }
}
