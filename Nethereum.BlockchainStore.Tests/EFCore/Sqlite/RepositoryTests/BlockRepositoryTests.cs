﻿using Nethereum.BlockchainStore.Tests.EFCore.Base.RepositoryTests;
using Nethereum.BlockchainStore.Tests.EFCore.Sqlite.Common;

namespace Nethereum.BlockchainStore.Tests.EFCore.Sqlite.RepositoryTests
{
    public class BlockRepositoryTests: BlockRepositoryBaseTests
    {
        public BlockRepositoryTests():base(Utils.CreateSqliteContextFactory())
        {
        }
    }
}