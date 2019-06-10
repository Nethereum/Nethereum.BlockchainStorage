using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System.Collections.Generic;
using Xunit;

namespace Nethereum.BlockchainProcessing.Tests.Processing.Logs
{
    public class LogExtensionTests
    {
        [Fact]
        public void FilterLog_Key_Generates_Expected_Value()
        {
            var populatedLog = new FilterLog {TransactionHash = "x", LogIndex = new HexBigInteger(0)};
            var emptyLog = new FilterLog();

            Assert.Equal("x0x0", populatedLog.Key());
            Assert.Equal(emptyLog.GetHashCode().ToString(), emptyLog.Key());
        }

        [Fact]
        public void FilterLog_Merge_Adds_Non_Duplicated_Candidate_Logs()
        {
            var log1 = new FilterLog {TransactionHash = "x", LogIndex = new HexBigInteger(0)};
            var dupe = new FilterLog {TransactionHash = "x", LogIndex = new HexBigInteger(0)};
            var log2 = new FilterLog {TransactionHash = "y", LogIndex = new HexBigInteger(0)};

            var masterList = new Dictionary<string, FilterLog>();
            masterList.Add(log1.Key(), log1);

            var candidates = new[] {dupe, log2};
            var merged = masterList.Merge(candidates);

            Assert.Same(merged, masterList);
            Assert.Contains(log1, masterList.Values);
            Assert.Contains(log2, masterList.Values);
            Assert.DoesNotContain(dupe, masterList.Values);
        }

        [Fact]
        public void Sort_By_Block_TxIndex_LogIndex()
        {
            var arbitrarilySortedLogs = new[] {
                CreateLog(2, 1, 3),
                CreateLog(2, 1, 2),
                CreateLog(2, 1, 1),

                CreateLog(1, 3, 3),
                CreateLog(1, 3, 2),
                CreateLog(1, 3, 1),

                CreateLog(1, 2, 3),
                CreateLog(1, 2, 2),
                CreateLog(1, 2, 1),

                CreateLog(1, 1, 3),
                CreateLog(1, 1, 2),
                CreateLog(1, 1, 1)
                };

            var sorted = arbitrarilySortedLogs.Sort();
            Assert.Equal(arbitrarilySortedLogs.Length, sorted.Length);


            VerifyLog(sorted[0], 1, 1, 1);
            VerifyLog(sorted[1], 1, 1, 2);
            VerifyLog(sorted[2], 1, 1, 3);
            VerifyLog(sorted[3], 1, 2, 1);
            VerifyLog(sorted[4], 1, 2, 2);
            VerifyLog(sorted[5], 1, 2, 3);
            VerifyLog(sorted[6], 1, 3, 1);
            VerifyLog(sorted[7], 1, 3, 2);
            VerifyLog(sorted[8], 1, 3, 3);
            VerifyLog(sorted[9], 2, 1, 1);
            VerifyLog(sorted[10], 2, 1, 2);
            VerifyLog(sorted[11], 2, 1, 3);

        }

        [Fact]
        public void Sort_Is_Null_Safe()
        {
            var logs = new[] {
                new FilterLog{BlockNumber = new HexBigInteger(1), TransactionIndex = new HexBigInteger(1), LogIndex = new HexBigInteger(1) },
                new FilterLog{BlockNumber = null, TransactionIndex = null, LogIndex = null}
                };

            var sorted = logs.Sort();
            Assert.Equal(logs.Length, sorted.Length);

            //nulls will be treat as zero and rise to the top
            Assert.Same(logs[1], sorted[0]);
            Assert.Same(logs[0], sorted[1]);
        }

        private void VerifyLog(FilterLog log, int expectedBlockNumber, int expectedTxIndex, int expectedLogIndex)
        {
            Assert.Equal(expectedBlockNumber, log.BlockNumber.Value);
            Assert.Equal(expectedTxIndex, log.TransactionIndex.Value);
            Assert.Equal(expectedLogIndex, log.LogIndex.Value);
        }

        private FilterLog CreateLog(int blockNumber, int txIndex, int logIndex)
        {
            return new FilterLog { BlockNumber = new HexBigInteger(blockNumber), TransactionIndex = new HexBigInteger(txIndex), LogIndex = new HexBigInteger(logIndex) };
        }
    }
}
