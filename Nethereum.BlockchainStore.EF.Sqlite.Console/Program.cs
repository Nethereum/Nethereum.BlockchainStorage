using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nethereum.BlockchainStore.Entities;
using Nethereum.BlockchainStore.Processing;

namespace Nethereum.BlockchainStore.EF.Sqlite.Console
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var contextFactory = new BlockchainDbContextFactory("BlockchainDbContext");
            var repositoryFactory = new BlockchainStoreRepositoryFactory(contextFactory);
            var presetName = args?.Length == 0 ? ProcessorConfigurationPresets.Default : args[0];
            var configuration = ProcessorConfigurationPresets.Get(presetName);
            return ProcessorConsole.Execute(args, repositoryFactory, configuration).Result;
        }
    }
}
