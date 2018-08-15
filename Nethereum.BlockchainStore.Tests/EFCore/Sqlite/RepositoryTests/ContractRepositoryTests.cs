﻿using Nethereum.BlockchainStore.Tests.EFCore.Base.RepositoryTests;
using Nethereum.BlockchainStore.Tests.EFCore.Sqlite.Common;

namespace Nethereum.BlockchainStore.Tests.EFCore.Sqlite.RepositoryTests
{
    public class ContractRepositoryTests: ContractRepositoryBaseTests
    {
        public ContractRepositoryTests():base(Utils.CreateSqliteContextFactory())
        {
        }
    }
}