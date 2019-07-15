using Nethereum.BlockchainStore.EF.EntityBuilders;
using System.Data.Entity;
//using Sap.Data.Hana; // Note: add this for access to the HANA data provider objects

namespace Nethereum.BlockchainStore.EF.Hana
{
    public class HanaBlockchainDbContext: BlockchainDbContextBase
    {
        public readonly string Schema;
        public readonly string ConnectionName;

        public HanaBlockchainDbContext(string connectionName, string schema) : base(connectionName)
        {
            Schema = schema;
            ConnectionName = connectionName;
            // Note: DefaultConnectionFactory is defined in App.config as HanaConnectionFactory();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {            
            modelBuilder.HasDefaultSchema(Schema);
            EntityBuilderExtensions.ColumnTypeForUnlimitedText = "nvarchar(max)";
            Database.SetInitializer(new CreateDatabaseIfNotExists<HanaBlockchainDbContext>());
            base.OnModelCreating(modelBuilder);
        }
    }
}
