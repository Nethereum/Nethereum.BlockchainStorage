﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nethereum.BlockchainStore.Entities;

namespace Nethereum.BlockchainStore.EF.SqlServer
{
    public class BlockchainDbContextFactory : IBlockchainDbContextFactory
    {
        static readonly Dictionary<DbSchemaNames, Type> SchemaToDbContextTypes;

        static BlockchainDbContextFactory()
        {
            SchemaToDbContextTypes = typeof(BlockchainDbContextFactory).Assembly.GetTypes()
                .Where(t => typeof(BlockchainDbContext).IsAssignableFrom(t))
                .Select(t => new {Type = t, Schema = (DbSchemaAttribute) t.GetCustomAttribute(typeof(DbSchemaAttribute))})
                .Where(o => o.Schema != null)
                .ToDictionary(el => el.Schema.DbSchemaName, el => el.Type);
        }

        private readonly string _connectionName;
        private readonly Type _typeOfContext;

        public BlockchainDbContextFactory(string connectionName, string schema): 
            this(connectionName, (DbSchemaNames)Enum.Parse(typeof(DbSchemaNames), schema)){}

        public BlockchainDbContextFactory(string connectionName, DbSchemaNames dbSchemaName)
        {
            _connectionName = connectionName;

            if(!SchemaToDbContextTypes.ContainsKey(dbSchemaName))
                throw new Exception($"Unsupported or unknown schema '{dbSchemaName}'.  Could not locate a BlockchainDbContext type based on the schema");

            _typeOfContext = SchemaToDbContextTypes[dbSchemaName];
        }

        public BlockchainDbContextBase CreateContext()
        {
            return (BlockchainDbContextBase) Activator.CreateInstance(_typeOfContext, new object[] {_connectionName});
        }
    }
}