using System.Collections.Generic;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{

    public abstract class DbRow
    {
        public long Id {get;set;}
    }

    public class SubscriberDto: DbRow
    {
        public bool Disabled {get;set;}
        public long PartitionId {get;set;}
        public string OrganisationName {get;set;}
    }

    public class EventSubscriptionDto: DbRow
    {
        public bool Disabled {get;set;}
        public long SubscriberId {get;set;}
        public long? ContractId {get;set;}

        public string EventSignature {get;set;}

    }

    public class DecodedEventHandlerDto : DbRow
    {
        public int Order {get;set;}
        public bool Disabled {get;set;}
        public long EventSubscriptionId {get;set;}

        // see EventHandlerType
        public string HandlerType {get;set;}
    }

    public class ContractDto: DbRow
    {
        public long SubscriberId {get;set;}
        public string Abi {get;set;}
        public string Name {get;set;}
    }

    public class EventAddressDto: DbRow
    {
        public long EventSubscriptionId {get;set;}
        public string Address {get;set;}

    }

    public class ParameterConditionDto: DbRow
    {
        public long EventSubscriptionId {get;set;}
        public int ParameterOrder {get;set;}
        public string Operator {get;set;}
        public string Value {get;set;}
    }

    public class EventSubscriptionStateDto : DbRow
    {
        public EventSubscriptionStateDto(){}

        public EventSubscriptionStateDto(long eventSubscriptionId)
        {
            EventSubscriptionId = eventSubscriptionId;
        }

        public Dictionary<string, object> Values {get;set; } = new Dictionary<string, object>();

        public long EventSubscriptionId { get; set; }

    }

    public class ContractQueryDto: DbRow
    {
        public long DecodedEventHandlerId {get;set;}

        public long ContractId {get;set;}

        public string FunctionSignature {get;set;} 

        public string[] OutputVariableNames {get;set;}

        public string[] ArgumentVariableNames {get;set;}

        public string ContractAddress {get; set;}

        public string ContractAddressVariableName {get;set;}

        public ContractAddressSource AddressLookupBehaviour {get;set;}

    }

    public enum ContractAddressSource
    {
        Static, EventAddress, EventParameter, EventMetaData
    }
}
