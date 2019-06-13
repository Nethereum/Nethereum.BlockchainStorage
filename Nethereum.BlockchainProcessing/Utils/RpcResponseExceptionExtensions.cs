using System;
using Nethereum.JsonRpc.Client;

namespace Nethereum.BlockchainProcessing
{
    public static class RpcResponseExceptionExtensions
    {
        public static readonly string TooManyRecordsMessagePrefix = "query returned more than";

        public static RpcResponseException CreateFakeTooManyRecordsRpcException()
        {
            return new RpcResponseException(new RpcError(0, TooManyRecordsMessagePrefix));
        }

        public static bool TooManyRecords(this RpcResponseException rpcResponseEx)
        {
            return rpcResponseEx.Message.StartsWith(TooManyRecordsMessagePrefix);
        }

        public static TooManyRecordsException TooManyRecordsException(this RpcResponseException rpcResponseEx)
        {
            return new TooManyRecordsException(rpcResponseEx);
        }
    }
}
