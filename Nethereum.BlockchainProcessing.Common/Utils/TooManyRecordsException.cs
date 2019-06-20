using System;
using Nethereum.JsonRpc.Client;

namespace Nethereum.BlockchainProcessing
{
    public class TooManyRecordsException: Exception
    {
        public TooManyRecordsException()
        {

        }

        public TooManyRecordsException(RpcResponseException innerException):base(innerException.Message, innerException)
        {

        }
    }
}
