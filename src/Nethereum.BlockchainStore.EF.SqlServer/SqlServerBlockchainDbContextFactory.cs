using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nethereum.BlockchainStore.Entities;

namespace Nethereum.BlockchainStore.EF.SqlServer
{
    public class SqlServerBlockchainDbContextFactory : IBlockchainDbContextFactory
    {
        static readonly Dictionary<DbSchemaNames, Type> SchemaToDbContextTypes;

        static SqlServerBlockchainDbContextFactory()
        {
            SchemaToDbContextTypes = typeof(SqlServerBlockchainDbContextFactory).Assembly.GetTypes()
                .Where(t => typeof(SqlServerBlockchainDbContext).IsAssignableFrom(t))
                .Select(t => new {Type = t, Schema = (DbSchemaAttribute) t.GetCustomAttribute(typeof(DbSchemaAttribute))})
                .Where(o => o.Schema != null)
                .ToDictionary(el => el.Schema.DbSchemaName, el => el.Type);
        }

        private readonly string _connectionName;
        private readonly Type _typeOfContext;

        public SqlServerBlockchainDbContextFactory(string connectionName, string schema): 
            this(connectionName, (DbSchemaNames)Enum.Parse(typeof(DbSchemaNames), schema)){}

        public SqlServerBlockchainDbContextFactory(string connectionName, DbSchemaNames dbSchemaName)
        {
            _connectionName = connectionName;

            if(!SchemaToDbContextTypes.ContainsKey(dbSchemaName))
                throw new Exception($"Unsupported or unknown schema '{dbSchemaName}'.  Could not locate a BlockchainDbContext type based on the schema");

            _typeOfContext = SchemaToDbContextTypes[dbSchemaName];
        }

        public BlockchainDbContextBase CreateContext()
        {
            return (SqlServerBlockchainDbContext) Activator.CreateInstance(_typeOfContext, new object[] {_connectionName});
        }
    }
}