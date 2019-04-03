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
    }
}
