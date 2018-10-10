using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Handlers
{
    public class TransactionVmStack
    {
        public TransactionVmStack(string transactionHash, string address, JObject stackTrace)
        {
            TransactionHash = transactionHash;
            Address = address;
            StackTrace = stackTrace;
        }

        public string TransactionHash { get; private set; }
        public string Address { get; private set; }
        public JObject StackTrace { get; private set; }
    }

    public interface ITransactionVMStackHandler
    {
        Task HandleAsync(TransactionVmStack transactionVmStack);
    }
}
