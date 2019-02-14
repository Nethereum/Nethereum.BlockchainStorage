using Nethereum.Contracts;

namespace Nethereum.BlockchainStore.Search
{
    public interface IEventFunctionProcessor<TFunction> : 
        IEventFunctionProcessor where TFunction : FunctionMessage, new() 
    {
    }
}
