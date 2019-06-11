using Nethereum.BlockchainStore.AzureTables.Bootstrap;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public static class LogsProcessingExtensions
    {
        public static ILogsProcessorBuilder UseAzureTableStorageForBlockProgress(
            this ILogsProcessorBuilder processorBuilder, string azureConnectionString, string tableNamePrefix)
        {
            return processorBuilder.UseAzureTableStorageForBlockProgress(new CloudTableSetup(azureConnectionString, tableNamePrefix));
        }

        public static ILogsProcessorBuilder UseAzureTableStorageForBlockProgress(
            this ILogsProcessorBuilder processor, CloudTableSetup cloudTableSetup)
        {
            processor.BlockProgressRepository = cloudTableSetup.CreateBlockProgressRepository();

            return processor;
        }

        public static ILogsProcessorBuilder StoreInAzureTable<TEventDto>(this ILogsProcessorBuilder processorBuilder, string azureConnectionString, string tablePrefix, Predicate<EventLog<TEventDto>> predicate = null)
            where TEventDto : class, new() => StoreInAzureTable<TEventDto>(processorBuilder, new CloudTableSetup(azureConnectionString, tablePrefix), predicate);

        public static ILogsProcessorBuilder StoreInAzureTable<TEventDto>(this ILogsProcessorBuilder processorBuilder, CloudTableSetup cloudTableSetup, Predicate<EventLog<TEventDto>> predicate = null)
            where TEventDto : class, new()
        {
            var transactionLogRepository = cloudTableSetup.CreateTransactionLogRepository();
            var processor = new TransactionLogProcessor<TEventDto>(transactionLogRepository, predicate);
            processorBuilder.Add(processor);
            return processorBuilder;
        }

        public static ILogsProcessorBuilder StoreInAzureTable(this ILogsProcessorBuilder processorBuilder, string azureConnectionString, string tablePrefix, Predicate<FilterLog> predicate = null)
            => StoreInAzureTable(processorBuilder, new CloudTableSetup(azureConnectionString, tablePrefix), predicate);

        public static ILogsProcessorBuilder StoreInAzureTable(this ILogsProcessorBuilder eventLogProcessor, CloudTableSetup cloudTableSetup, Predicate<FilterLog> predicate = null)
        {
            var transactionLogRepository = cloudTableSetup.CreateTransactionLogRepository();
            var processor = new TransactionLogProcessor(transactionLogRepository, predicate);
            eventLogProcessor.Add(processor);
            return eventLogProcessor;
        }
    }
}
