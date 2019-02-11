using Nethereum.Contracts;
using System;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public interface IAzureFunctionSearchIndex<TFunctionMessage> : 
        IDisposable, 
        IFunctionIndexer<TFunctionMessage>, 
        IAzureSearchIndex
        where TFunctionMessage : FunctionMessage, new()
    {

    }

}