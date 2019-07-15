using System;

namespace Nethereum.BlockchainStore.Entities
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