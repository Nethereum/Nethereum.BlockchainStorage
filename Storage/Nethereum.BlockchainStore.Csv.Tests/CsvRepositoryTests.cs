using Nethereum.BlockchainStore.Test.Base.RepositoryTests;
using System;
using System.IO;
using Xunit;

namespace Nethereum.BlockchainStore.Csv.Tests
{

    public class CsvTestFixture : IDisposable
    {
        public static readonly string CsvPath = Path.Combine(Path.GetTempPath(), "BlockchainStorageCsvTests", "csv");

        public CsvTestFixture()
        {
            if (!Directory.Exists(CsvPath))
                Directory.CreateDirectory(CsvPath);

            DeleteCsvFiles();

            Factory = new CsvBlockchainStoreRepositoryFactory(CsvPath);
        }

        private static void DeleteCsvFiles()
        {
            foreach (var file in Directory.GetFiles(CsvPath, "*.csv"))
            {
                File.Delete(file);
            }
        }

        public CsvBlockchainStoreRepositoryFactory Factory { get; }

        public void Dispose()
        {
            DeleteCsvFiles();
        }
    }

    [CollectionDefinition("CsvTestFixture")]
    public class CsvTestFixtureCollection : ICollectionFixture<CsvTestFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }

    [Collection("CsvTestFixture")]
    public class CsvRepositoryTests: RepositoryLayerTestBase
    {
        public CsvRepositoryTests(CsvTestFixture fixture) : base(fixture.Factory){}

    }
}
