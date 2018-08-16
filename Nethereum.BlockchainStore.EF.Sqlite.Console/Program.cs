using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nethereum.BlockchainStore.Entities;

namespace Nethereum.BlockchainStore.EF.Sqlite.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var context = new BlockchainDbContext("BlockchainDbContext1");
            context.Blocks.Add(new Block {BlockNumber = "1", Hash = "hash", ParentHash = "Parent "});
            context.SaveChanges();
        }
    }
}
