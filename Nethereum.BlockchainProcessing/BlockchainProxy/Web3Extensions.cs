using Nethereum.Geth;
using Nethereum.Geth.RPC.Debug.DTOs;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.Web3
{
    internal class VmStackMockResults 
    {
        public Dictionary<string, JObject> MockVmStacks = new Dictionary<string, JObject>();
    }

    internal class VmStackMockExceptions
    {
        public Dictionary<string, Exception> MockVmExceptions = new Dictionary<string, Exception>();
    }


    public static class Web3Extensions
    {
        // a hack to work around the fact that only Geth supports retrieving the stack trace
        // this is to allow unit testing to insert mocks
        
        static Dictionary<IWeb3, VmStackMockResults> _mockVmStackResults = new Dictionary<IWeb3, VmStackMockResults>();
        static Dictionary<IWeb3, VmStackMockExceptions> _mockVmStackExceptions = new Dictionary<IWeb3, VmStackMockExceptions>();

        public static void ClearVmStackMocks(this IWeb3 web3)
        {
            _mockVmStackExceptions.Remove(web3);
            _mockVmStackResults.Remove(web3);
        }

        public static void SetupMockForGetTransactionVmStack(this IWeb3 web3, string transactionHash, JObject vmStack)
        {
            if (!_mockVmStackResults.ContainsKey(web3))
            {
                _mockVmStackResults.Add(web3, new VmStackMockResults());
            }

            _mockVmStackResults[web3].MockVmStacks[transactionHash] = vmStack;
        }

        public static void SetupMockForGetTransactionVmStack(this IWeb3 web3, string transactionHash, Exception exceptionToThrow)
        {
            if (!_mockVmStackExceptions.ContainsKey(web3))
            {
                _mockVmStackExceptions.Add(web3, new VmStackMockExceptions());
            }

            _mockVmStackExceptions[web3].MockVmExceptions[transactionHash] = exceptionToThrow;
        }

        public static JObject GetMockedTransactionVmStack(this IWeb3 web3, string transactionHash)
        {
            if(_mockVmStackExceptions.ContainsKey(web3) && _mockVmStackExceptions[web3].MockVmExceptions.TryGetValue(transactionHash, out Exception ex))
            {
                _mockVmStackExceptions[web3].MockVmExceptions.Remove(transactionHash);
                throw ex;
            }

            if(_mockVmStackResults.ContainsKey(web3) && _mockVmStackResults[web3].MockVmStacks.TryGetValue(transactionHash, out JObject vmStack))
            {
                _mockVmStackResults[web3].MockVmStacks.Remove(transactionHash);
                return vmStack;
            }
            return null;
        }

        public static async Task<JObject> GetTransactionVmStack(this IWeb3 web3, string transactionHash)
        {

            if (web3 is Web3Geth web3Geth)
                return await web3Geth.Debug.
                    TraceTransaction.
                    SendRequestAsync(
                        transactionHash,
                        new TraceTransactionOptions { DisableMemory = true, DisableStorage = true, DisableStack = false })
                    .ConfigureAwait(false);

            return GetMockedTransactionVmStack(web3, transactionHash);
        }
    }
}
