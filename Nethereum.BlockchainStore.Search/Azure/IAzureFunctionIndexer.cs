using Nethereum.Contracts;
using System;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public interface IAzureFunctionIndexer<TFunctionMessage> : 
        IDisposable, 
        IFunctionIndexer<TFunctionMessage>, 
        IAzureIndex
        where TFunctionMessage : FunctionMessage, new()
    {

    }

}