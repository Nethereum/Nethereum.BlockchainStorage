using System;

namespace Nethereum.BlockchainStore.EFCore.SqlServer
{
    public class DbSchemaAttribute: Attribute
    {
        public DbSchemaNames DbSchemaName { get; }

        public DbSchemaAttribute(DbSchemaNames dbSchemaName)
        {
            DbSchemaName = dbSchemaName;
        }
    }
}