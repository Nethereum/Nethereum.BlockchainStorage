namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public static class ConfigurationDtoExtensions
    {
        public static EventSubscriptionDto CreateEventSubscription(this SubscriberDto subscriber, long id, long? contractId = null, string eventSignature = null)
        {
            return new EventSubscriptionDto
            {
                Id = id,
                SubscriberId = subscriber.Id,
                Disabled = false,
                ContractId = contractId,
                EventSignature = eventSignature
            };
        } 
        
        public static ContractDto CreateContract(this SubscriberDto subscriber, long id, string name, string abi)
        {
            return new ContractDto
            {
                Id = id,
                Name = name,
                SubscriberId = subscriber.Id,
                Abi = abi
            };
        }
        
        public static EventAddressDto CreateEventAddress(this EventSubscriptionDto eventSubscription, int id, string address)
        {
            return new EventAddressDto
            {
                Id = id,
                Address = address,
                EventSubscriptionId = eventSubscription.Id
            };
        }
    }
}
