using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.Model;
using Nethereum.Contracts;
using Nethereum.Contracts.Extensions;
using Nethereum.RPC.Eth.DTOs;
using System.Collections.Generic;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public static class ParameterOutputExtensions
    {
       public static DecodedEvent  ToDecodedEvent(this FilterLog log, EventABI abi)
        {
            var decoded = abi?.DecodeEventDefaultTopics(log) ?? new EventLog<List<ParameterOutput>>(new List<ParameterOutput>(), log);
            return new DecodedEvent{Log = decoded};
        }
    }
}
