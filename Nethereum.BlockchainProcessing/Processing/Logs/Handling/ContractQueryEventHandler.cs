using Nethereum.BlockchainProcessing.BlockchainProxy;
using System.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{

    public class ContractQueryEventHandler : IDecodedEventHandler
    {
        public ContractQueryEventHandler(IContractQuery contractQueryProxy, EventSubscriptionStateDto state, ContractQueryConfiguration queryConfig)
        {
            Proxy = contractQueryProxy ?? throw new System.ArgumentNullException(nameof(contractQueryProxy));
            Config = queryConfig ?? throw new System.ArgumentNullException(nameof(queryConfig));
            State = state ?? throw new System.ArgumentNullException(nameof(state));
        }

        public IContractQuery Proxy { get; }
        public ContractQueryConfiguration Config { get; }
        public EventSubscriptionStateDto State { get; }

        public async Task<bool> HandleAsync(DecodedEvent decodedEvent)
        {
            var contractAddress = GetContractAddress(decodedEvent);
            if(contractAddress == null) return false;

            var functionInputs = BuildFunctionInputs(decodedEvent);

            var result = await Proxy.Query(contractAddress, Config.ContractABI, Config.FunctionSignature, functionInputs);

            if(!string.IsNullOrEmpty(Config.EventStateOutputName))
            {
                decodedEvent.State[Config.EventStateOutputName] = result;
            }

            if (!string.IsNullOrEmpty(Config.SubscriptionStateOutputName))
            {
                State.Set(Config.SubscriptionStateOutputName, result);
            }

            return true;
        }

        private object[] BuildFunctionInputs(DecodedEvent decodedEvent)
        {
            if(Config.Parameters == null || Config.Parameters.Length == 0)
            {
                return null;
            }
            object[] parameterValues = new object[Config.Parameters.Length];

            var ordered = Config.Parameters.OrderBy(p => p.Order).ToArray();

            for(int i = 0; i < ordered.Length; i++)
            {
                parameterValues[i] = GetFunctionInputParameterValue(ordered[i], decodedEvent);
            }

            return parameterValues;
        }

        private object GetFunctionInputParameterValue(ContractQueryParameter functionParameter, DecodedEvent decodedEvent)
        {
            switch (functionParameter.Source)
            {
                case EventValueSource.Static:
                    return functionParameter.Value;
                case EventValueSource.EventAddress:
                    return decodedEvent.EventLog.Log.Address;
                case EventValueSource.EventParameters:
                    return GetFunctionInputFromEventParameter(functionParameter, decodedEvent);
                case EventValueSource.EventState:
                    return GetFunctionInputFromMetadata(functionParameter, decodedEvent);
                default:
                    return null;
            }
        }

        private static object GetFunctionInputFromMetadata(ContractQueryParameter functionParameter, DecodedEvent decodedEvent)
        {
            if(string.IsNullOrEmpty(functionParameter.EventStateName)) return null;

            return decodedEvent.State.ContainsKey(functionParameter.EventStateName) ?
                                    decodedEvent.State[functionParameter.EventStateName] : null;
        }

        private static object GetFunctionInputFromEventParameter(ContractQueryParameter functionParameter, DecodedEvent decodedEvent)
        {
            if(functionParameter.EventParameterNumber < 1) return null;

            return decodedEvent.EventLog.Event.FirstOrDefault(p => p.Parameter.Order == functionParameter.EventParameterNumber )?.Result;
        }

        private string GetContractAddress(DecodedEvent decodedEvent)
        {
            switch (Config.ContractAddressSource)
            {
                case ContractAddressSource.Static:
                    return Config.ContractAddress;
                case ContractAddressSource.EventAddress:
                    return decodedEvent.EventLog.Log.Address;
                case ContractAddressSource.EventParameter:
                    return GetAddressFromEventParameter(decodedEvent);
                case ContractAddressSource.EventState:
                    return GetAddressFromEventMetaData(decodedEvent);
                default:
                    return null;

            }
        }

        private string GetAddressFromEventParameter(DecodedEvent decodedEvent)
        {
            if(Config.ContractAddressParameterNumber > 0)
            {
                var matchingEventParameter = decodedEvent.EventLog.Event.FirstOrDefault(p => p.Parameter.Order == Config.ContractAddressParameterNumber);
                if(matchingEventParameter != null && matchingEventParameter.Result is string a)
                {
                    return a;
                }
            }
            return null;
        }

        private string GetAddressFromEventMetaData(DecodedEvent decodedEvent)
        {
            if(!string.IsNullOrEmpty(Config.ContractAddressStateVariableName))
            {
                var addressFromMetaData = decodedEvent.State.ContainsKey(Config.ContractAddressStateVariableName) ? 
                    decodedEvent.State[Config.ContractAddressStateVariableName] : null;

                if(addressFromMetaData is string mS)
                {
                    return mS;
                }
            }
            return null;
        }
    }
}
