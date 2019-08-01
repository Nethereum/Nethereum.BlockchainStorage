//using Nethereum.Contracts;
//using System;

//namespace Nethereum.BlockchainStore.Search
//{
//    public class FunctionMessageToSearchDocumentMapper<TFunctionMessage, TSearchDocument> : 
//        IFunctionMessageToSearchDocumentMapper<TFunctionMessage, TSearchDocument>

//        where TFunctionMessage : FunctionMessage, new() where TSearchDocument : class, new()
//    {
//        private readonly Func<FunctionCall<TFunctionMessage>, TSearchDocument> _mappingFunc;

//        public FunctionMessageToSearchDocumentMapper(Func<FunctionCall<TFunctionMessage>, TSearchDocument> mappingFunc)
//        {
//            _mappingFunc = mappingFunc;
//        }

//        public TSearchDocument Map(FunctionCall<TFunctionMessage> from)
//        {
//            return _mappingFunc.Invoke(from);
//        }
//    }
//}
