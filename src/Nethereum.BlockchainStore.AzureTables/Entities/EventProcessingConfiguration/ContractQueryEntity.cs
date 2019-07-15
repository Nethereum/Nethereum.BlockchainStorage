using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using System;

namespace Nethereum.BlockchainStore.AzureTables.Entities.EventProcessingConfiguration
{
    public class ContractQueryEntity : TableEntity, IContractQueryDto
    {
        public long Id
        {
            get => this.RowKeyToLong();
            set => RowKey = value.ToString();
        }

        public long EventHandlerId 
        {
            get => this.PartionKeyToLong();
            set => PartitionKey = value.ToString();
        }

        public string ContractAddressSourceAsString
        {
            get
            {
                return ContractAddressSource.ToString();
            }
            set
            {
                ContractAddressSource = (ContractAddressSource)Enum.Parse(typeof(ContractAddressSource), value);
            }
        }

        public ContractAddressSource ContractAddressSource { get; set; }
        public long ContractId { get; set; }
        public string FunctionSignature { get; set; }
        public string ContractAddress { get; set; }
        public int? ContractAddressParameterNumber { get; set; }
        public string ContractAddressStateVariableName { get; set; }
        public string EventStateOutputName { get; set; }
        public string SubscriptionStateOutputName {get;set; }

    }
}
