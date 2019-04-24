using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using System.Collections.Generic;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Configuration
{

    public interface IDbRow
    {
        long Id { get; set; }
    }

    public interface ISubscriberDto : IDbRow
    {
         bool Disabled { get; set; }
         long PartitionId { get; set; }
         string Name { get; set; }
    }

    public interface ISubscriberContractDto : IDbRow
    {

        long SubscriberId { get; set; }

        string Abi { get; set; }

        string Name { get; set; }
    }

    public abstract class DbRow: IDbRow
    {
        public long Id {get;set;}
    }

    public class SubscriberDto: DbRow, ISubscriberDto
    {
        public bool Disabled {get;set;}
        public long PartitionId {get;set;}
        public string Name {get;set;}
    }

    public class EventSubscriptionDto : DbRow, IEventSubscriptionDto
    {
        public bool Disabled { get; set; }
        public long SubscriberId { get; set; }
        public long? ContractId { get; set; }

        public bool CatchAllContractEvents { get; set; }
        public List<string> EventSignatures { get; set; } = new List<string>();

    }

    public interface IEventHandlerDto : IDbRow
    {
        int Order { get; set; }
        bool Disabled { get; set; }
        long EventSubscriptionId { get; set; }

        EventHandlerType HandlerType { get; set; }

        long SubscriberQueueId { get; set; }

        long SubscriberSearchIndexId { get; set; }

        long SubscriberRepositoryId { get; set; }
    }

    public class EventHandlerDto : DbRow, IEventHandlerDto
    {
        public int Order {get;set;}
        public bool Disabled {get;set;}
        public long EventSubscriptionId {get;set;}

        public EventHandlerType HandlerType {get;set;}

        public long SubscriberQueueId {get;set;}

        public long SubscriberSearchIndexId {get;set;}

        public long SubscriberRepositoryId { get;set;}
    }

    public class SubscriberContractDto: DbRow, ISubscriberContractDto
    {
        public long SubscriberId {get;set;}
        public string Abi {get;set;}
        public string Name {get;set;}
    }

    public interface IEventSubscriptionAddressDto : IDbRow
    {
        long EventSubscriptionId { get; set; }
        string Address { get; set; }
    }

    public class EventSubscriptionAddressDto: DbRow, IEventSubscriptionAddressDto
    {
        public long EventSubscriptionId {get;set;}
        public string Address {get;set;}

    }

    public interface IParameterConditionDto : IDbRow
    {
        long EventSubscriptionId { get; set; }
        int ParameterOrder { get; set; }
        ParameterConditionOperator Operator { get; set; }
        string Value { get; set; }
    }

    public class ParameterConditionDto: DbRow, IParameterConditionDto
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

    public interface IEventSubscriptionStateDto : IDbRow
    {

        Dictionary<string, object> Values { get; set; }

        long EventSubscriptionId { get; set; }

    }

    public class EventSubscriptionStateDto : DbRow, IEventSubscriptionStateDto
    {
        public EventSubscriptionStateDto(){}

        public EventSubscriptionStateDto(long eventSubscriptionId)
        {
            EventSubscriptionId = eventSubscriptionId;
        }

        public Dictionary<string, object> Values {get;set; } = new Dictionary<string, object>();

        public long EventSubscriptionId { get; set; }

    }

    public interface IContractQueryDto : IDbRow
    {
        long EventHandlerId { get; set; }

        ContractAddressSource ContractAddressSource { get; set; }

        long ContractId { get; set; }

        string FunctionSignature { get; set; }

        string ContractAddress { get; set; }

        int? ContractAddressParameterNumber { get; set; }

        string ContractAddressStateVariableName { get; set; }

        string EventStateOutputName { get; set; }
        string SubscriptionStateOutputName { get; set; }

    }


    public class ContractQueryDto: DbRow, IContractQueryDto
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
    public interface IContractQueryParameterDto : IDbRow
    {
        
        long ContractQueryId { get; set; }
        
        int Order { get; set; }
        
        EventValueSource Source { get; set; }
        
        object Value { get; set; }
        
        int EventParameterNumber { get; set; }
        
        string EventStateName { get; set; }
    }

    public class ContractQueryParameterDto: DbRow, IContractQueryParameterDto
    {
        public long ContractQueryId {get;set;}
        public int Order {get;set;}
        public EventValueSource Source {get;set;}
        public object Value {get;set;}
        public int EventParameterNumber {get;set;}
        public string EventStateName {get;set;}
    }

    public interface IEventAggregatorDto : IDbRow
    {
        long EventHandlerId { get; set; }
        AggregatorSource Source { get; set; }
        AggregatorOperation Operation { get; set; }
        AggregatorDestination Destination { get; set; }
        int EventParameterNumber { get; set; }
        string SourceKey { get; set; }
        string OutputKey { get; set; }
    }

    public class EventAggregatorDto: DbRow, IEventAggregatorDto
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

    public interface ISubscriberQueueDto : IDbRow
    {
        bool Disabled { get; set; }
        long SubscriberId { get; set; }
        string Name { get; set; }
    }

    public class SubscriberQueueDto: DbRow, ISubscriberQueueDto
    {
        public bool Disabled {get;set;}
        public long SubscriberId {get;set;}
        public string Name {get;set;}
    }

    public interface ISubscriberSearchIndexDto : IDbRow
    {
        bool Disabled { get; set; }
        long SubscriberId { get; set; }
        string Name { get; set; }
    }

    public class SubscriberSearchIndexDto: DbRow, ISubscriberSearchIndexDto
    {
        public bool Disabled {get;set;}
        public long SubscriberId {get;set;}
        public string Name {get;set;}
    }

    public interface IEventHandlerHistoryDto : IDbRow
    {
        long EventHandlerId { get; set; }
        string EventKey { get; set; }

        long SubscriberId { get; set; }
        long EventSubscriptionId { get; set; }
    }

    public class EventHandlerHistoryDto: DbRow, IEventHandlerHistoryDto
    {
        public long SubscriberId { get; set; }

        public long EventSubscriptionId { get; set; }
        public long EventHandlerId {get;set;}
        public string EventKey { get;set;} 
    }

    public interface ISubscriberStorageDto : IDbRow
    {
        bool Disabled { get; set; }
        long SubscriberId { get; set; }
        string Name { get; set; }
    }

    public class SubscriberStorageDto : DbRow, ISubscriberStorageDto
    {
        public bool Disabled { get; set; }
        public long SubscriberId { get; set; }
        public string Name { get; set; }
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

    public interface IEventRuleDto : IDbRow
    {
        EventRuleSource Source { get; set; }
        EventRuleType Type { get; set; }

        string InputName { get; set; }

        int EventParameterNumber { get; set; }

        string Value { get; set; }

        long EventHandlerId { get;set;}
    }

    public class EventRuleDto: DbRow, IEventRuleDto
    {
        public long EventHandlerId { get;set;}
        public EventRuleSource Source { get; set; }
        public EventRuleType Type { get; set; }

        public string InputName { get; set; }

        public int EventParameterNumber { get; set; }

        public string Value { get; set; }
    }

}
