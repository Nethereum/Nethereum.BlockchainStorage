﻿using Microsoft.EntityFrameworkCore;
using Nethereum.BlockchainProcessing.BlockStorage.Entities;
using System;
using System.Reflection;

namespace Nethereum.BlockchainStore.EFCore.SqlServer
{
    [DbSchema(DbSchemaNames.dbo)]
    public class SqlServerBlockchainDbContext_dbo: SqlServerBlockchainDbContext
    {
        public SqlServerBlockchainDbContext_dbo(string connectionString) : base(connectionString){}
    }

    [DbSchema(DbSchemaNames.localhost)]
    public class SqlServerBlockchainDbContext_localhost : SqlServerBlockchainDbContext
    {
        public SqlServerBlockchainDbContext_localhost(string connectionString) : base(connectionString){}
    }

    [DbSchema(DbSchemaNames.rinkeby)]
    public class SqlServerBlockchainDbContext_rinkeby : SqlServerBlockchainDbContext
    {
        public SqlServerBlockchainDbContext_rinkeby(string connectionString) : base(connectionString){}
    }

    [DbSchema(DbSchemaNames.kovan)]
    public class SqlServerBlockchainDbContext_kovan : SqlServerBlockchainDbContext
    {
        public SqlServerBlockchainDbContext_kovan(string connectionString) : base(connectionString){}
    }

    [DbSchema(DbSchemaNames.main)]
    public class SqlServerBlockchainDbContext_main : SqlServerBlockchainDbContext
    {
        public SqlServerBlockchainDbContext_main(string connectionString) : base(connectionString){}
    }

    [DbSchema(DbSchemaNames.ropsten)]
    public class SqlServerBlockchainDbContext_ropsten : SqlServerBlockchainDbContext
    {
        public SqlServerBlockchainDbContext_ropsten(string connectionString) : base(connectionString){}
    }

    [DbSchema(DbSchemaNames.goerli)]
    public class SqlServerBlockchainDbContext_goerli : SqlServerBlockchainDbContext
    {
        public SqlServerBlockchainDbContext_goerli(string connectionString) : base(connectionString) { }
    }

    [DbSchema(DbSchemaNames.sepolia)]
    public class SqlServerBlockchainDbContext_sepolia : SqlServerBlockchainDbContext
    {
        public SqlServerBlockchainDbContext_sepolia(string connectionString) : base(connectionString) { }
    }

    public abstract class SqlServerBlockchainDbContext: BlockchainDbContextBase
    {
        private readonly string _connectionString;

        public string Schema { get; }

        protected SqlServerBlockchainDbContext(string connectionString)
        {
            ColumnTypeForUnlimitedText = "nvarchar(max)";

            _connectionString = connectionString;

            var dbSchemaAttribute = (DbSchemaAttribute)this.GetType().GetCustomAttribute(typeof(DbSchemaAttribute));
            if(dbSchemaAttribute == null)
                throw new Exception("Type requires a DBSchema custom attribute");

            Schema = dbSchemaAttribute.DbSchemaName.ToString();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Schema);
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString, (a) =>
            {
                
            });
        }
    }
}
