using System;
using System.Collections.Generic;
using System.Text;

namespace Nethereum.BlockchainStore.EFCore.Tests.RepositoryTests
{
    public class RepositoryTestBase
    {
        protected readonly IBlockchainDbContextFactory contextFactory;

        public RepositoryTestBase(IBlockchainDbContextFactory contextFactory)
        {
            this.contextFactory = contextFactory;
        }
    }
}
