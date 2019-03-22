using Nethereum.ABI.FunctionEncoding;
using Nethereum.Contracts;
using System.Collections.Generic;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Matching
{
    /// <summary>
    /// abstraction to allow filters with different operators (=, >, < etc)
    /// </summary>
    public interface IParameterCondition
    {
        bool IsTrue(EventLog<List<ParameterOutput>> eventLog);
    }
}
