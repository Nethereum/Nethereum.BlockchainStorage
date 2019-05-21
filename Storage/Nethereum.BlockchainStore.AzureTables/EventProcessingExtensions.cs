using Nethereum.BlockchainStore.AzureTables.Bootstrap;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public static class EventProcessingExtensions
    {
        public static IEventLogProcessor UseAzureTableStorageForBlockProgress(
            this IEventLogProcessor processor, string azureConnectionString, string tableNamePrefix)
        {
            return processor.UseAzureTableStorageForBlockProgress(new CloudTableSetup(azureConnectionString, tableNamePrefix));
        }

        public static IEventLogProcessor UseAzureTableStorageForBlockProgress(
            this IEventLogProcessor processor, CloudTableSetup cloudTableSetup)
        {
            processor.BlockProgressRepository = cloudTableSetup.CreateBlockProgressRepository();

            return processor;
        }

        public static IEventLogProcessor StoreInAzureTable<TEventDto>(this IEventLogProcessor eventLogProcessor, string azureConnectionString, string tablePrefix, Predicate<EventLog<TEventDto>> predicate = null)
            where TEventDto : class, new() => StoreInAzureTable<TEventDto>(eventLogProcessor, new CloudTableSetup(azureConnectionString, tablePrefix), predicate);

        public static IEventLogProcessor StoreInAzureTable<TEventDto>(this IEventLogProcessor eventLogProcessor, CloudTableSetup cloudTableSetup, Predicate<EventLog<TEventDto>> predicate = null)
            where TEventDto : class, new()
        {
            var transactionLogRepository = cloudTableSetup.CreateTransactionLogRepository();
            var processor = new TransactionLogProcessor<TEventDto>(transactionLogRepository, predicate);
            eventLogProcessor.Subscribe(processor);
            return eventLogProcessor;
        }

        public static IEventLogProcessor StoreInAzureTable(this IEventLogProcessor eventLogProcessor, string azureConnectionString, string tablePrefix, Predicate<FilterLog> predicate = null)
            => StoreInAzureTable(eventLogProcessor, new CloudTableSetup(azureConnectionString, tablePrefix), predicate);

        public static IEventLogProcessor StoreInAzureTable(this IEventLogProcessor eventLogProcessor, CloudTableSetup cloudTableSetup, Predicate<FilterLog> predicate = null)
        {
            var transactionLogRepository = cloudTableSetup.CreateTransactionLogRepository();
            var processor = new TransactionLogProcessor(transactionLogRepository, predicate);
            eventLogProcessor.Subscribe(processor);
            return eventLogProcessor;
        }
    }
}
