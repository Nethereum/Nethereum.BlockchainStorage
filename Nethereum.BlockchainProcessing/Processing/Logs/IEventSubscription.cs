namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public interface IEventSubscription: ILogProcessor
    {
        long Id {get;}
        long SubscriberId {get;}
    }
}
