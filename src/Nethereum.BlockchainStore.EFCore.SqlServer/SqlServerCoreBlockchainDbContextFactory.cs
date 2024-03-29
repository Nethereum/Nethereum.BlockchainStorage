﻿using Microsoft.Extensions.Configuration;
using Nethereum.BlockchainProcessing.BlockStorage.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nethereum.BlockchainStore.EFCore.SqlServer
{
    public class SqlServerCoreBlockchainDbContextFactory : IBlockchainDbContextFactory
    {
        public const string DbName = "BlockchainDbStorage";

        static readonly Dictionary<string, Type> SchemaToDbContextTypes;

        static SqlServerCoreBlockchainDbContextFactory()
        {
            SchemaToDbContextTypes = typeof(SqlServerCoreBlockchainDbContextFactory).Assembly.GetTypes()
                .Where(t => typeof(SqlServerBlockchainDbContext).IsAssignableFrom(t))
                .Select(t => new {Type = t, Schema = (DbSchemaAttribute) t.GetCustomAttribute(typeof(DbSchemaAttribute))})
                .Where(o => o.Schema != null)
                .ToDictionary(el => el.Schema.DbSchemaName, el => el.Type);
        }

        public static SqlServerCoreBlockchainDbContextFactory Create(IConfigurationRoot config)
        {
            var schema = config.GetBlockchainStorageDbSchema();
            var connectionString = config.GetBlockchainStorageConnectionString(schema);
            return new SqlServerCoreBlockchainDbContextFactory(connectionString, schema);
        }

        private readonly string _connectionString;
        private readonly Type _typeOfContext;

        public SqlServerCoreBlockchainDbContextFactory(string connectionString, string dbSchemaName)
        {
            _connectionString = connectionString;

            if (!SchemaToDbContextTypes.ContainsKey(dbSchemaName))
                throw new Exception($"Unsupported or unknown schema '{dbSchemaName}'.  Could not locate a BlockchainDbContext type based on the schema");

            _typeOfContext = SchemaToDbContextTypes[dbSchemaName];
        }

        public SqlServerCoreBlockchainDbContextFactory(string connectionString, DbSchemaNames dbSchemaName):this(connectionString, dbSchemaName.ToString())
        {
           
        }

        public BlockchainDbContextBase CreateContext()
        {
            return (BlockchainDbContextBase) Activator.CreateInstance(_typeOfContext, new object[] {_connectionString});
        }
    }
}