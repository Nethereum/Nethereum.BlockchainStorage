using Nethereum.ABI.Model;
using Nethereum.Contracts;
using System;
using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.Handlers;

namespace Nethereum.BlockchainProcessing.InMemory.Console
{
    public class FunctionPrinter<TFunctionInput>: ITransactionHandler where TFunctionInput : FunctionMessage, new()
    {
        private readonly FunctionABI _functionAbi;

        public FunctionPrinter()
        {
            _functionAbi = ABITypedRegistry.GetFunctionABI<TFunctionInput>();

            if(_functionAbi == null)
                throw new ArgumentException("Function for TFunctionInput not found in contract");
        }

        public Task HandleContractCreationTransactionAsync(ContractCreationTransaction contractCreationTransaction)
        {
            return Task.CompletedTask;
        }

        public Task HandleTransactionAsync(TransactionWithReceipt txnWithReceipt)
        {
            if(!txnWithReceipt.IsForFunction<TFunctionInput>())
                return Task.CompletedTask;

            var dto = txnWithReceipt.Decode<TFunctionInput>();

            System.Console.WriteLine($"[FUNCTION]");
            System.Console.WriteLine($"\t{_functionAbi.Name ?? "unknown"}");
   
            foreach (var prop in dto.GetType().GetProperties())
            {
                System.Console.WriteLine($"\t\t[{prop.Name}:{prop.GetValue(dto) ?? "null"}]");
            }

            return Task.CompletedTask;
        }
    }
}
