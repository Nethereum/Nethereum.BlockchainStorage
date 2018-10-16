using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nethereum.Hex.HexTypes;
using Xunit;

namespace Nethereum.BlockchainStore.Tests
{
    public class GetTransactionSpeedTests
    {
        //[Fact]
        public async Task GetBlocksWithTransactions()
        {
            const int startingBlock = 3131645;
            const int endBlock = startingBlock + 50;

            var web3 = new Web3.Web3("https://rinkeby.infura.io/v3/25e7b6dfc51040b3bfc0e47317d38f60");

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            for (var block = startingBlock; block <= endBlock; block++)
            {
                await web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(new HexBigInteger(block));
            }

            var allInOne_Elapsed = stopWatch.Elapsed;
            Debug.WriteLine($"All In One Duration: {allInOne_Elapsed}" );

            stopWatch.Reset();
            stopWatch.Start();

            for (var block = startingBlock; block <= endBlock; block++)
            {
                var result = await web3.Eth.Blocks.GetBlockWithTransactionsHashesByNumber.SendRequestAsync(new HexBigInteger(block));
                foreach (var tx in result.TransactionHashes)
                {
                    var t = await web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(tx);
                }
            }

            var multipleRequests_Elapsed = stopWatch.Elapsed;
            Debug.WriteLine($"Multiple Requests Duration: {multipleRequests_Elapsed}" );

            /*
             RINKEBY
                10 Blocks
                     All In One Duration: 00:00:02.2257667
                     Multiple Requests Duration: 00:00:14.1144150

                50 Blocks
                    All In One Duration: 00:00:10.0810022
                    Multiple Requests Duration: 00:01:31.8176943

             */
        }
    }
}
