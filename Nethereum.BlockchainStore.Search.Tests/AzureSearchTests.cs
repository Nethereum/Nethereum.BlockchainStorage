using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainStore.Search.Azure;
using Nethereum.BlockchainStore.Search;
using Nethereum.Configuration;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Xunit;

namespace Nethereum.BlockchainStore.Search.Tests
{
    public class AzureSearchTests
    {
        [Event("Transfer")]
        public class TransferEvent : IEventDTO
        {
            [Parameter("address", "_from", 1, true)]
            public string From {get; set;}

            [Parameter("address", "_to", 2, true)]
            public string To {get; set;}

            [Parameter("uint256", "_value", 3, true)]
            public BigInteger Value {get; set;}
        }

        public const string ApiKeyName = "AzureSearchApiKey";
        public const string AzureSearchServiceName = "blockchainsearch";

        [Fact]
        public async Task CreateIndexAndUpsert()
        {
            ConfigurationUtils.SetEnvironment("development");

            var appConfig = ConfigurationUtils
                .Build(Array.Empty<string>(), userSecretsId: "Nethereum.BlockchainStore.Search.Tests");

            var apiKey = appConfig[ApiKeyName];

            var transferSearchIndex = new EventSearchIndexDefinition<TransferEvent>();

            using (var searchService = new AzureSearchService(AzureSearchServiceName, apiKey))
            {
                try
                {
                    var index = await searchService.CreateIndexAsync(transferSearchIndex);
                    Assert.NotNull(index);

                    var transferEventLog = new EventLog<TransferEvent>(
                        new TransferEvent
                        {
                            From = "0x9209b29f2094457d3dba62d1953efea58176ba27",
                            To = "0x1209b29f2094457d3dba62d1953efea58176ba28",
                            Value = new HexBigInteger("2000000")
                        },
                        new FilterLog
                        {
                            Address = "0x26bc47888b7bfdf77db41ec0a2fb4db00af1c92a",
                            TransactionHash = "0xcb00b69d2594a3583309f332ada97d0df48bae00170e36a4f7bbdad7783fc7e5",
                            BlockNumber = new HexBigInteger(7118507),
                            BlockHash = "0x337cd6feedafac6abba40eff40fb1957e08985180f5a03016924ef72fc7b04b9",
                            LogIndex = new HexBigInteger(0),
                            Removed = false,
                            TransactionIndex = new HexBigInteger(0)
                        });

                    await searchService.UpsertAsync(transferSearchIndex, transferEventLog);
                }
                finally
                {
                    await searchService.DeleteIndexAsync(transferSearchIndex);
                }
            }
        }
    }
}
