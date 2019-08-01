using Nethereum.Contracts;
using System;

namespace Nethereum.BlockchainStore.Search
{
    public abstract class FunctionIndexer<TFunctionMessage, TSearchDocument> : IndexerBase<FunctionCall<TFunctionMessage>, TSearchDocument> where TFunctionMessage : FunctionMessage, new() where TSearchDocument : class
    {
        protected FunctionIndexer(Func<FunctionCall<TFunctionMessage>, TSearchDocument> mapper, int logsPerIndexBatch) : base(mapper, logsPerIndexBatch) { }
    }

}
