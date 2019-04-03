namespace Nethereum.BlockchainProcessing.Handlers
{
    public class HandlerContainer
    {
        public virtual IBlockHandler BlockHandler { get; set;} = new NullBlockHandler();
        public virtual ITransactionHandler TransactionHandler { get; set;} = new NullTransactionHandler();
        public virtual ITransactionLogHandler TransactionLogHandler { get; set;} = new NullTransactionLogHandler();
        public virtual ITransactionVMStackHandler TransactionVmStackHandler { get; set;} = new NullTransactionVMStackHandler();
        public virtual IContractHandler ContractHandler { get; set;} = new NullContractHandler();
    }
}
