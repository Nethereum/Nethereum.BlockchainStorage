using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
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
        public string Name {get;set;}
    }

    public class EventSubscriptionDto: DbRow
    {
        public bool Disabled {get;set;}
        public long SubscriberId {get;set;}
        public long? ContractId {get;set;}

        public string EventSignature {get;set;}

    }

    public class EventHandlerDto : DbRow
    {
        public int Order {get;set;}
        public bool Disabled {get;set;}
        public long EventSubscriptionId {get;set;}

        public EventHandlerType HandlerType {get;set;}

        public long SubscriberQueueId {get;set;}

        public long SubscriberSearchIndexId {get;set;}
    }

    public class SubscriberContractDto: DbRow
    {
        public long SubscriberId {get;set;}
        public string Abi {get;set;}
        public string Name {get;set;}
    }

    public class EventSubscriptionAddressDto: DbRow
    {
        public long EventSubscriptionId {get;set;}
        public string Address {get;set;}

    }

    public class ParameterConditionDto: DbRow
    {
        public long EventSubscriptionId {get;set;}
        public int ParameterOrder {get;set;}
        public ParameterConditionOperator Operator {get;set;}
        public string Value {get;set;}
    }

    public enum ParameterConditionOperator
    {
        Equals, GreaterOrEqual, LessOrEqual
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
        public long EventHandlerId {get;set;}

        public ContractAddressSource ContractAddressSource {get;set; }

        public long ContractId {get;set;}

        public string FunctionSignature {get;set;}

        public string ContractAddress {get; set;}

        public int? ContractAddressParameterNumber {get;set;}

        public string ContractAddressStateVariableName {get;set;}

        public string EventStateOutputName {get;set;}
        public string SubscriptionStateOutputName {get;set;}

    }

    public class ContractQueryParameterDto: DbRow
    {
        public long ContractQueryId {get;set;}
        public int Order {get;set;}
        public EventValueSource Source {get;set;}
        public object Value {get;set;}
        public int EventParameterNumber {get;set;}
        public string EventStateName {get;set;}
    }

    public class EventAggregatorConfigurationDto: DbRow
    {
        public long EventHandlerId {get;set;}
        public AggregatorSource Source {get;set;}
        public AggregatorOperation Operation {get;set;}
        public AggregatorDestination Destination {get;set;}
        public int EventParameterNumber {get;set;}
        public string SourceKey {get;set;}
        public string OutputKey {get;set;}
    }

    public enum ContractAddressSource
    {
        Static, EventAddress, EventParameter, EventState, TransactionFrom, TransactionTo
    }

    public enum EventValueSource
    {
        Static, EventAddress, EventParameters, EventState
    }

    public enum AggregatorDestination
    {
        EventState, EventSubscriptionState
    }

    public enum AggregatorOperation
    {
        //max, latest, min??
        Count, Sum, AddToList
    }

    public enum AggregatorSource
    {
        EventParameter, EventState, TransactionHash, BlockNumber
    }

    public class SubscriberQueueConfigurationDto: DbRow
    {
        public bool Disabled {get;set;}
        public long SubscriberId {get;set;}
        public string Name {get;set;}
    }

    public class SubscriberSearchIndexConfigurationDto: DbRow
    {
        public bool Disabled {get;set;}
        public long SubscriberId {get;set;}
        public string Name {get;set;}
    }

    public class EventHandlerHistoryDto: DbRow
    {
        public long SubscriberId {get;set; }
        public long EventSubscriptionId {get;set; }
        public long EventHandlerId {get;set; }
        public string EventKey { get;set;}
    }

    public class EventRuleConfigurationDto: DbRow
    {
        public long EventHandlerId { get;set;}

        public EventRuleSource Source { get; set; }
        public EventRuleType Type { get; set; }

        public string SourceKey { get; set; }

        public int EventParameterNumber { get; set; }

        public string Value { get; set; }
    }

    public enum EventRuleSource
    {
        Static, EventParameter, EventState, EventSubscriptionState
    }

    public enum EventRuleType
    {
        Equals, GreaterOrEqualTo, LessThanOrEqualTo, Modulus, Empty
    }

}
