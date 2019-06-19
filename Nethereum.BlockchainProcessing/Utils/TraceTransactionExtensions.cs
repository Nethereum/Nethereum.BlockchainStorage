using Nethereum.Geth;
using Nethereum.Geth.RPC.Debug.DTOs;
using Nethereum.Web3;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processors.Transactions
{
    public static class TraceTransactionExtensions
    {
        static ConcurrentDictionary<IWeb3, Func<string, JObject>> _getTransactionVmStackInterceptors = new ConcurrentDictionary<IWeb3, Func<string, JObject>>();

        // a hook for unit test interception
        // GetTransactionVmStack is currently Web3Geth specific
        // this allows a unit test to simulate Web3Geth
        public static void RegisterGetVmStackInterceptor(this IWeb3 web3, Func<string, JObject> interceptionFunc) 
            => _getTransactionVmStackInterceptors.AddOrUpdate(web3, interceptionFunc, (w3, i) => interceptionFunc);

        public static void RemoveGetVmStackInterceptor(this IWeb3 web3) => _getTransactionVmStackInterceptors.TryRemove(web3, out _);

        private static JObject GetInterceptedTransactionVmStackResult(IWeb3 web3, string transactionHash)
        {
            if(_getTransactionVmStackInterceptors.TryGetValue(web3, out Func<string, JObject> interceptor))
            {
                return interceptor.Invoke(transactionHash);
            }
            return null;
        }

        /// <summary>
        /// For most purposes this is for Web3Geth only
        /// If web3 is Web3Geth call web3Geth.Debug.TraceTransaction
        /// Otherwise call registered interceptors (mainly for unit tests) else null
        /// </summary>
        public static async Task<JObject> GetTransactionVmStack(this IWeb3 web3, string transactionHash)
        {
            var interceptedResult = GetInterceptedTransactionVmStackResult(web3, transactionHash);
            if(interceptedResult != null) return interceptedResult;

            if (web3 is Web3Geth web3Geth)
                return await web3Geth.Debug.
                    TraceTransaction.
                    SendRequestAsync(
                        transactionHash,
                        new TraceTransactionOptions { DisableMemory = true, DisableStorage = true, DisableStack = false })
                    .ConfigureAwait(false);

            return null;
        }
    }
}
