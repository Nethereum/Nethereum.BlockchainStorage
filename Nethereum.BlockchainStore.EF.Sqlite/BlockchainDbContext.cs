using Nethereum.BlockchainStore.EF.EntityBuilders;
using SQLite.CodeFirst;
using System.Data.Entity;

namespace Nethereum.BlockchainStore.EF.Sqlite
{
    public class BlockchainDbContext: BlockchainDbContextBase
    {
        public BlockchainDbContext(string connectionName):base(connectionName){}

        public BlockchainDbContext()
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            EntityBuilderExtensions.ColumnTypeForUnlimitedText = "nvarchar";
            var sqliteConnectionInitializer = new SqliteCreateDatabaseIfNotExists<BlockchainDbContext>(modelBuilder);
            Database.SetInitializer(sqliteConnectionInitializer);
            base.OnModelCreating(modelBuilder);
        }
    }
}
