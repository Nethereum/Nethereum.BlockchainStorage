using System;
using System.Collections.Generic;
using System.Text;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.BlockchainStore.Test.Base.RepositoryTests;
using Xunit;

namespace Nethereum.BlockchainStore.AzureTables.Tests.RepositoryTests
{
    [Collection("AzureTablesFixture")]
    public class AzureTablesRepositoryTests : RepositoryLayerTestBase
    {
        public AzureTablesRepositoryTests(AzureTablesFixture fixture) : base(fixture.Factory)
        {
        }
    }
}
