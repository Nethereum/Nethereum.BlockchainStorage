using Nethereum.BlockchainProcessing.BlockchainProxy;
using System.Linq;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{

    public class ContractQuery : IDecodedEventHandler
    {
        public ContractQuery(IContractQuery contractQueryProxy, EventSubscriptionStateDto state, ContractQueryConfiguration queryConfig)
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

            if(!string.IsNullOrEmpty(Config.MetaDataOutputName))
            {
                decodedEvent.Metadata[Config.MetaDataOutputName] = result;
            }

            if (!string.IsNullOrEmpty(Config.EventSuscriptionStateVariableName))
            {
                State.Set(Config.EventSuscriptionStateVariableName, result);
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
                case EventValueSource.EventParameters:
                    return GetFunctionInputFromEventParameter(functionParameter, decodedEvent);
                case EventValueSource.EventMetaData:
                    return GetFunctionInputFromMetadata(functionParameter, decodedEvent);
                default:
                    return null;
            }
        }

        private static object GetFunctionInputFromMetadata(ContractQueryParameter functionParameter, DecodedEvent decodedEvent)
        {
            if(string.IsNullOrEmpty(functionParameter.EventMetadataName)) return null;

            return decodedEvent.Metadata.ContainsKey(functionParameter.EventMetadataName) ?
                                    decodedEvent.Metadata[functionParameter.EventMetadataName] : null;
        }

        private static object GetFunctionInputFromEventParameter(ContractQueryParameter functionParameter, DecodedEvent decodedEvent)
        {
            if(functionParameter.EventParameterNumber < 1) return null;

            return decodedEvent.Log.Event.FirstOrDefault(p => p.Parameter.Order == functionParameter.EventParameterNumber )?.Result;
        }

        private string GetContractAddress(DecodedEvent decodedEvent)
        {
            switch (Config.ContractAddressSource)
            {
                case ContractAddressSource.Static:
                    return Config.ContractAddress;
                case ContractAddressSource.EventAddress:
                    return decodedEvent.Log.Log.Address;
                case ContractAddressSource.EventParameter:
                    return GetAddressFromEventParameter(decodedEvent);
                case ContractAddressSource.EventMetaData:
                    return GetAddressFromEventMetaData(decodedEvent);
                default:
                    return null;

            }
        }

        private string GetAddressFromEventParameter(DecodedEvent decodedEvent)
        {
            if(Config.ContractAddressParameterNumber > 0)
            {
                var matchingEventParameter = decodedEvent.Log.Event.FirstOrDefault(p => p.Parameter.Order == Config.ContractAddressParameterNumber);
                if(matchingEventParameter != null && matchingEventParameter.Result is string a)
                {
                    return a;
                }
            }
            return null;
        }

        private string GetAddressFromEventMetaData(DecodedEvent decodedEvent)
        {
            if(!string.IsNullOrEmpty(Config.ContractAddressMetadataVariableName))
            {
                var addressFromMetaData = decodedEvent.Metadata.ContainsKey(Config.ContractAddressMetadataVariableName) ? 
                    decodedEvent.Metadata[Config.ContractAddressMetadataVariableName] : null;

                if(addressFromMetaData is string mS)
                {
                    return mS;
                }
            }
            return null;
        }
    }
}
