namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling.Handlers
{
    public interface IQueueMessageMapper
    {
        object Map(DecodedEvent decodedEvent);
    }

}
