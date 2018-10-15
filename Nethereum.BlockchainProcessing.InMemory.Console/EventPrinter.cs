using Nethereum.BlockchainStore.Handlers;
using Nethereum.Contracts;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.InMemory.Console
{
    public class EventPrinter<TEvent>: ITransactionLogHandler where TEvent: new()
    {
        private readonly string _eventName;

        public EventPrinter()
        {
            _eventName = ABITypedRegistry.GetEvent<TEvent>().Name;
        }

        public Task HandleAsync(TransactionLogWrapper transactionLog)
        {
            if (!transactionLog.IsForEvent<TEvent>())
                return Task.CompletedTask;

            var eventValues = transactionLog.Decode<TEvent>();
            if (eventValues == null) return Task.CompletedTask;

            System.Console.WriteLine($"[EVENT]");
            System.Console.WriteLine($"\t[{_eventName}]");
            foreach (var prop in eventValues.GetType().GetProperties())
            {
                System.Console.WriteLine($"\t\t[{prop.Name}:{prop.GetValue(eventValues) ?? "null"}]");
            }

            return Task.CompletedTask;
        }
    }
}
