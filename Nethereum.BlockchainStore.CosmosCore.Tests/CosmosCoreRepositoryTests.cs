using Nethereum.BlockchainStore.CosmosCore.Bootstrap;
using Nethereum.BlockchainStore.EFCore;
using Nethereum.BlockchainStore.Test.Base.RepositoryTests;

namespace Nethereum.BlockchainStore.CosmosCore.Tests
{
    public class CosmosCoreRepositoryTests: RepositoryLayerTestBase
    {
        public static readonly string[] CommandLineArgs = new string[] {};
        public static readonly string UserSecretsId = "Nethereum.BlockchainStore.CosmosCore.UserSecrets.UnitTests";

        static CosmosCoreRepositoryTests()
        {
            ConfigurationUtils.SetEnvironment("development");
        }

        public CosmosCoreRepositoryTests() : base(CosmosRepositoryFactory.Create(
            CommandLineArgs, 
            UserSecretsId, 
            deleteAllExistingCollections: true))
        {
        }
    }
}
