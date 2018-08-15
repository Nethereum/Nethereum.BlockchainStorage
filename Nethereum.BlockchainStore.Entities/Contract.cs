﻿namespace Nethereum.BlockchainStore.Entities
{
    public class Contract: TableRow
    {
        public string Address { get; set; }
        public string Name { get; set; }
        public string ABI { get; set; }
        public string Code { get; set; }
        public string Creator { get;set; }
        public string TransactionHash { get; set; }
    }
}