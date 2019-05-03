using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.Model;
using Nethereum.Contracts;
using Nethereum.Contracts.Extensions;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public static class DecodedEventExtensions
    {
        public static DecodedEvent ToDecodedEvent(this FilterLog log, EventABI abi = null)
        {
            var decodedParameterOutputs = abi?.DecodeEventDefaultTopics(log) ?? 
                new EventLog<List<ParameterOutput>>(new List<ParameterOutput>(), log);

            var decodedEvent = new DecodedEvent(decodedParameterOutputs.Event, decodedParameterOutputs.Log);
            decodedEvent.AddStateData(abi, log);
            return decodedEvent;
        }

        public static DecodedEvent ToDecodedEvent<TEvent>(this FilterLog log, EventABI abi) where TEvent: new()
        {
            var decodedParameterOutputs = abi.DecodeEventDefaultTopics(log);

            var decodedDto = log.DecodeEvent<TEvent>();

            var decodedEvent = new DecodedEvent(decodedParameterOutputs.Event, decodedParameterOutputs.Log, decodedDto.Event);
            decodedEvent.AddStateData(abi, log);
            return decodedEvent;
        }

        private static void AddStateData(this DecodedEvent decodedEvent, EventABI abi, FilterLog log)
        {
            decodedEvent.State["EventAbiName"] = abi?.Name;
            decodedEvent.State["EventSignature"] = abi?.Sha3Signature;
            decodedEvent.State["TransactionHash"] = log.TransactionHash;
            decodedEvent.State["LogIndex"] = log.LogIndex?.Value;
            decodedEvent.State["HandlerInvocations"] = 0;
            decodedEvent.State["UtcNowMs"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
    }
}
