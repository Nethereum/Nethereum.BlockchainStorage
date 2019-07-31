using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;

namespace Nethereum.BlockchainProcessing.Queue
{
    public class EventLogQueueMessage
    {
        public string Key { get; set; }

        /// <summary>
        /// An instance of the decoded Event DTO (optional)
        /// </summary>
        public object Event { get; set; }

        /// <summary>
        /// Additional state data that may have accumulated or calculated during processing
        /// </summary>
        public Dictionary<string, object> State { get; set; }

        /// <summary>
        /// An instance of the transaction related to the log (optional)
        /// </summary>
        public Transaction Transaction { get; set; }

        /// <summary>
        /// The event topic values aka parameters or event arguments
        /// </summary>
        public List<EventParameterValue> ParameterValues { get; set; }

        public FilterLog Log { get; set; }

    }
}
