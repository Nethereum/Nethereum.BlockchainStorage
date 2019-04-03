using System;
using System.Collections.Generic;
using System.Linq;
using Nethereum.BlockchainProcessing.Processing;

namespace Nethereum.BlockchainProcessing.Processors.Transactions
{
    public class ContractSpecificFilterBuilder
    {
        public ITransactionFilter TransactionFilter { get; }
        public ITransactionReceiptFilter TransactionReceiptFilter { get; }
        public FilterContainer Filters { get; }

        public ContractSpecificFilterBuilder(IEnumerable<string> contractAddresses)
        {
            if(contractAddresses == null) throw new ArgumentNullException(nameof(contractAddresses));

            var addresses = contractAddresses.ToArray();

            TransactionFilter = Transactions.TransactionFilter.ToOrEmpty(addresses);
            TransactionReceiptFilter = Transactions.TransactionReceiptFilter.ForContractOrEmpty(addresses);

            Filters = new FilterContainer(
                TransactionFilter,
                TransactionReceiptFilter); 
        }

        public ContractSpecificFilterBuilder(string contractAddress):this(new []{contractAddress}){}
    }
}
