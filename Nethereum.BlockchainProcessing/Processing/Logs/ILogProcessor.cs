using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.Model;
using Nethereum.Contracts;
using Nethereum.Contracts.Extensions;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public interface ILogProcessor
    {
        bool IsLogForEvent(FilterLog log);
        Task ProcessLogsAsync(params FilterLog[] eventLogs);
    }

    public interface ILogProcessorFactory
    {
        ILogProcessor[] GetLogProcessors();
    }


    public class EventLogMatchingCriteria
    {
        public EventABI Abi {get;set;}

        public List<EventLogParameterFilter> ParameterFilters {get;set;}

        public List<string> Addresses {get;set;}

        public bool IsMatch(FilterLog log)
        {
            if(Abi != null)
            {
                if(!log.IsLogForEvent(Abi.Sha3Signature)) return false;
            }

            if(Addresses?.Count > 0)
            {
                if(!Addresses.Contains(log.Address, StringComparer.OrdinalIgnoreCase)) return false;
            }

            if(Abi != null && (ParameterFilters?.Any() ?? false))
            {
                var topics = Abi.DecodeEventDefaultTopics(log);
                var matchingFilters = ParameterFilters.Where(f => f.IsMatch(topics)).ToArray();
                if(matchingFilters.Length != ParameterFilters.Count)
                {
                    return false;
                }
            }

            return true;
        }
        
    }

    public class EventLogParameterFilter
    {
        public string ParameterName {get;set;}
        public string Value {get;set;}

        public bool IsMatch(EventLog<List<ParameterOutput>> eventLog)
        {
            var parameter = eventLog.Event.FirstOrDefault(p => p.Parameter.Name == ParameterName);
            if(parameter == null) return false;

            if(parameter.Result == null)
            {
                return string.IsNullOrEmpty(Value);
            }

            var parameterValueAsString = parameter.Result.ToString();

            if(string.IsNullOrEmpty(Value) && string.IsNullOrEmpty(parameterValueAsString))
            {
                return true;
            }

            return Value == parameter.Result.ToString();
        }
    }


    public class LogProcessor : ILogProcessor
    {
        private readonly EventLogMatchingCriteria matchingCriteria;

        public LogProcessor(EventLogMatchingCriteria config)
        {
            matchingCriteria = config;
        }

        public bool IsLogForEvent(FilterLog log)
        {
            return matchingCriteria.IsMatch(log);
        }

        public Task ProcessLogsAsync(params FilterLog[] eventLogs)
        {
            return Task.CompletedTask;
        }
    }

    public class LogProcessorFactory : ILogProcessorFactory
    {
        public LogProcessorFactory(string partitionId = null)
        {

        }

        public ILogProcessor[] GetLogProcessors()
        {
            /*
             * bool IsLogForEvent(FilterLog log);
             * Detector rules
             * log for address
             * event sig match
             * event value match
             * aggregate rule
             * 
             * ProcessLogsAsync(params FilterLog[] eventLogs);
             * Actions to take on matching logs
             * 
             * Data to serialize/deserialize to hydrate processor
             * event abi
             * addresses
             * 
             * Do something with the event
             * Update aggregates
             * Push to queue - raw event or map it to something?
             * Notifications
             * 
             */ 
            
             
    
            throw new NotImplementedException();
        }
    }
}
