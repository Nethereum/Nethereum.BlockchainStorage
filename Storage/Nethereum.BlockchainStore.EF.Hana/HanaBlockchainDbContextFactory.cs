using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nethereum.BlockchainStore.Entities;

namespace Nethereum.BlockchainStore.EF.Hana
{
    public class HanaBlockchainDbContextFactory : IBlockchainDbContextFactory
    {        
        private readonly string _connectionName;
        private readonly string _schema;
        
        public HanaBlockchainDbContextFactory(string connectionName, string schema)
        {
            _connectionName = connectionName;
            _schema = schema;
        }

        public BlockchainDbContextBase CreateContext()
        {
            return new HanaBlockchainDbContext(_connectionName, _schema);
        }
    }
}