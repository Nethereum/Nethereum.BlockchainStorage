using System;
using System.Collections.Generic;
using System.Text;
using Nethereum.Hex.HexTypes;

namespace Nethereum.BlockchainStore.SqlServer.Tests
{
    public static class Utils
    {
        public const string connectionString =
            "Data Source=davewhiffin.database.windows.net;Database=BlockchainStorage;Integrated Security=False;User ID=localhost1;Password=MeLLfMA1wBlJCzSGZhkO;Connect Timeout=60;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public static BlockchainDbContextFactory CreateDbContextFactory(string schema = "localhost")
        {
            return  new BlockchainDbContextFactory(Utils.connectionString, schema);
        }

        public static HexBigInteger CreateBlockTimestamp()
        {
            return new HexBigInteger(DateTimeOffset.UnixEpoch.ToUnixTimeSeconds());
        }
    }
}
