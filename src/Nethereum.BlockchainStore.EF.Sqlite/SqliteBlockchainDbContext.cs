using Nethereum.BlockchainStore.EF.EntityBuilders;
using SQLite.CodeFirst;
using System.Data.Entity;

namespace Nethereum.BlockchainStore.EF.Sqlite
{
    public class SqliteBlockchainDbContext: BlockchainDbContextBase
    {
        public SqliteBlockchainDbContext(string connectionName):base(connectionName){}

        public SqliteBlockchainDbContext()
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            EntityBuilderExtensions.ColumnTypeForUnlimitedText = "nvarchar";
            var sqliteConnectionInitializer = new SqliteCreateDatabaseIfNotExists<SqliteBlockchainDbContext>(modelBuilder);
            Database.SetInitializer(sqliteConnectionInitializer);
            base.OnModelCreating(modelBuilder);
        }
    }
}
