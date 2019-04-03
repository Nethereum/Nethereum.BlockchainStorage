using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.Processing;
using Xunit;

namespace Nethereum.BlockchainProcessing.Tests.Processing.Logs
{
    public class JsonBlockProgressRepositoryTests
    {
        private string CreateJsonFilePath()
        {
            return Path.ChangeExtension(Path.GetTempFileName(), "json");
        }

        [Fact]
        public async Task Persists_And_Returns_Last_Block_Processed()
        {
            var filePath = CreateJsonFilePath();
            DeleteFile(filePath);
            try
            {
                var repo = new JsonBlockProgressRepository(filePath);
                Assert.Null(await repo.GetLastBlockNumberProcessedAsync());

                await repo.UpsertProgressAsync((ulong)1);
                Assert.Equal((ulong)1, await repo.GetLastBlockNumberProcessedAsync());

                repo = new JsonBlockProgressRepository(filePath);
                Assert.Equal((ulong)1, await repo.GetLastBlockNumberProcessedAsync());

            }
            finally
            {
                DeleteFile(filePath);
            }
        }

        private static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
