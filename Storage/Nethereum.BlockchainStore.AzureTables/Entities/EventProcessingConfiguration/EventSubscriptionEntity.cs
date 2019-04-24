using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Processing.Logs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nethereum.BlockchainStore.AzureTables.Entities.EventProcessingConfiguration
{
    public class EventSubscriptionEntity : TableEntity, IEventSubscriptionDto
    {
        public bool CatchAllContractEvents {get;set; }
        public long? ContractId { get; set; }
        public bool Disabled { get; set; }
        public List<string> EventSignatures { get; set; }

        /// <summary>
        /// Azure Table Storage does not allow storage of generic lists
        /// This property allows the list to be stored and retrieved as a delimeted string
        /// </summary>
        public string EventSignaturesCsv
        {
            get
            {
                return EventSignatures == null ? string.Empty : string.Join(",", EventSignatures);
            }
            set
            {
                EventSignatures = value.Split(new []{',' }).ToList();
            }
        }

        public long Id
        {
            get => this.RowKeyToLong();
            set => RowKey = value.ToString();
        }

        public long SubscriberId
        {
            get => this.PartionKeyToLong();
            set => PartitionKey = value.ToString();
        }
    }
}
