using System.Collections.Generic;

namespace Nethereum.BlockchainStore.Search
{
    public class TransactionReceiptVOIndexDefinition : IndexDefinition
    {
        public TransactionReceiptVOIndexDefinition(string indexName, bool addStandardBlockchainFields = true) : base(indexName, addStandardBlockchainFields)
        {
        }

        protected override void LoadGenericBlockchainFields()
        {
            PresetSearchFields.AddPresetTransactionFields(FieldDictionary);
        }
    }
}