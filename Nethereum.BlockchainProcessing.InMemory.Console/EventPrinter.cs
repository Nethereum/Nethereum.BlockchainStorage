using Nethereum.BlockchainStore.Handlers;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.InMemory.Console
{
    public class EventPrinter<T>: ITransactionLogHandler where T: new()
    {
        private readonly string _eventName;
        private readonly Contracts.Event _event;

        public EventPrinter(Contracts.Contract contract, string eventName)
        {
            _eventName = eventName;
            _event = contract.GetEvent(eventName);
        }

        public Task HandleAsync(TransactionLogWrapper transactionLog)
        {
            var eventValues = transactionLog.DecodeEvent<T>(_event);
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
