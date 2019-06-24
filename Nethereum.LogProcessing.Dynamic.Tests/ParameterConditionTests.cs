using Nethereum.ABI.FunctionEncoding;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Matching;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Util;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Xunit;

namespace Nethereum.LogProcessing.Tests
{
    public class ParameterConditionTests
    {
        public class ParameterGreaterOrEqualTests
        {
            [Fact]
            public void BigIntegers()
            {
                var actualValue = BigInteger.One;

                EventLog<List<ParameterOutput>> eventLog = EventLogWithParameter(1, "uint256", actualValue);

                var condition = new ParameterGreaterOrEqual(1, "1");
                Assert.True(condition.IsTrue(eventLog));

                condition = new ParameterGreaterOrEqual(1, "2");
                Assert.False(condition.IsTrue(eventLog));

                condition = new ParameterGreaterOrEqual(1, "0");
                Assert.True(condition.IsTrue(eventLog));
            }
        }

        public class ParameterLessOrEqualTests
        {
            [Fact]
            public void BigIntegers()
            {
                var actualValue = BigInteger.One;

                EventLog<List<ParameterOutput>> eventLog = EventLogWithParameter(1, "uint256", actualValue);

                var condition = new ParameterLessOrEqual(1, "1");
                Assert.True(condition.IsTrue(eventLog));

                condition = new ParameterLessOrEqual(1, "2");
                Assert.True(condition.IsTrue(eventLog));

                condition = new ParameterLessOrEqual(1, "0");
                Assert.False(condition.IsTrue(eventLog));
            }
        }

        public class ParameterEqualsTests
        {

            [Fact]
            public void IsSensitiveToParameterOrder()
            {
                EventLog<List<ParameterOutput>> eventLog = EventLogWithParameter(2, "string", "magic");

                var condition = new ParameterEquals(1, "magic");
                Assert.False(condition.IsTrue(eventLog));
            }

            [Fact]
            public void Bytes32ToString()
            {
                var hashString = new Sha3Keccack().CalculateHash("magic");
                var hashBytes = Encoding.UTF8.GetBytes(hashString);

                EventLog<List<ParameterOutput>> eventLog = EventLogWithParameter(1, "bytes32", hashBytes);

                var condition = new ParameterEquals(1, hashString);
                Assert.True(condition.IsTrue(eventLog));
            }

            [Fact]
            public void Strings()
            {
                EventLog<List<ParameterOutput>> eventLog = EventLogWithParameter(1, "string", "magic");

                var condition = new ParameterEquals(1, "magic");
                Assert.True(condition.IsTrue(eventLog));
            }

            [Fact]
            public void CaseSensitiveStrings()
            {
                EventLog<List<ParameterOutput>> eventLog = EventLogWithParameter(1, "string", "magic");

                var condition = new ParameterEquals(1, "Magic", caseSensitive: true);
                Assert.False(condition.IsTrue(eventLog));
            }

            [Fact]
            public void CaseInSensitiveStrings()
            {
                EventLog<List<ParameterOutput>> eventLog = EventLogWithParameter(1, "string", "magic");

                var condition = new ParameterEquals(1, "Magic", caseSensitive: false);
                Assert.True(condition.IsTrue(eventLog));
            }

            [Fact]
            public void BigIntegers()
            {
                EventLog<List<ParameterOutput>> eventLog = EventLogWithParameter(1, "uint256", BigInteger.One);

                var condition = new ParameterEquals(1, "1");
                Assert.True(condition.IsTrue(eventLog));
            }

            [Fact]
            public void HexBigIntegers()
            {
                EventLog<List<ParameterOutput>> eventLog = EventLogWithParameter(1, "uint256", new HexBigInteger(BigInteger.One));

                var condition = new ParameterEquals(1, "1");
                Assert.True(condition.IsTrue(eventLog));
            }



        }

        private static EventLog<List<ParameterOutput>> EventLogWithParameter(int parameterOrder, string type, object val)
        {
            var parameterValues = new List<ParameterOutput>
            {
                new ParameterOutput
                {
                    Parameter = new ABI.Model.Parameter(type, parameterOrder),
                    Result = val
                }
            };

            var log = new FilterLog();
            var eventLog = new EventLog<List<ParameterOutput>>(parameterValues, log);
            return eventLog;
        }
    }
}
