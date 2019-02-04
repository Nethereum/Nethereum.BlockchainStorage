using System;
using System.Linq;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Xunit;
using Xunit.Sdk;

namespace Nethereum.BlockchainStore.Search.Tests
{
    public class FunctionSearchIndexDefinitionTests
    {
        [Function("Deposit")]
        public class DepositDto
        {
            //configure a primary key
            [SearchField(IsKey = true, IsSearchable = true)]
            [Parameter("bytes32", "_id", 1)]
            public string Id { get;set; }

            //customise how the field is indexed
            [SearchField(IsFacetable = true, IsFilterable = true)]
            [Parameter("bytes32", "_currency", 1)]
            public string Currency { get;set; }

            //this value will be saved in the index however it won't be searchable by default
            [Parameter("uint256", "_amount", 2)]
            public uint Amount { get; set; }

            [SearchField(Ignore = true)]
            [Parameter("string", "_approver", 3)]
            public string Approver { get; set; }

            //not related to solidity args (in or out)
            [SearchField(IsFilterable = true, IsFacetable = true)]
            public string Category { get;set; }
        }

        [Fact]
        public void DefinesExpectedName()
        {
            var searchIndex = new FunctionSearchIndexDefinition<DepositDto>(addPresetTransactionFields:false);
            var dto = new DepositDto();

            Assert.Equal("Deposit", searchIndex.IndexName);
        }

        [Fact]
        public void CanIncludeNonParameterFieldsWithSearchFieldAttribute()
        {
            var searchIndex = new FunctionSearchIndexDefinition<DepositDto>(addPresetTransactionFields:false);
            var dto = new DepositDto{Category = "test"};

            var categoryField = searchIndex.Field(nameof(dto.Category));
            Assert.True(categoryField.IsFacetable);
            Assert.True(categoryField.IsFilterable);
            Assert.Equal(dto.Category, categoryField.GetValue(dto));
        }

        [Fact]
        public void ParameterFieldsCanBeIgnored()
        {
            var searchIndex = new FunctionSearchIndexDefinition<DepositDto>(addPresetTransactionFields:false);
            var dto = new DepositDto();

            Assert.Null(searchIndex.Field(nameof(dto.Approver)));
        }

        [Fact]
        public void APrimaryKeyFieldCanBeConfigured()
        {
            var searchIndex = new FunctionSearchIndexDefinition<DepositDto>(addPresetTransactionFields:false);
            var dto = new DepositDto();

            var idField = searchIndex.Field(nameof(dto.Id));
            Assert.True(idField.IsKey);
            Assert.True(idField.IsSearchable);
            Assert.Equal(dto.Id, idField.GetValue(dto));
        }

        [Fact]
        public void AllowsSearchCustomisationOfParameterFields()
        {
            var searchIndex = new FunctionSearchIndexDefinition<DepositDto>(addPresetTransactionFields:false);
            var dto = new DepositDto();

            var currencyField = searchIndex.Field(nameof(dto.Currency));
            Assert.Equal(dto.Currency, currencyField.GetValue(dto));
            Assert.True(currencyField.IsFacetable);
            Assert.True(currencyField.IsFilterable);
        }

        [Fact]
        public void WillAddParametersToSearchData_ButWillNotBeSearchableByDefault()
        {
            var searchIndex = new FunctionSearchIndexDefinition<DepositDto>(addPresetTransactionFields:false);
            var dto = new DepositDto();

            var amountField = searchIndex.Field(nameof(dto.Amount));
            Assert.Equal(dto.Amount, amountField.GetValue(dto));
            Assert.False(amountField.IsSearchable);
        }

        public class EmptyDto{}

        [Fact]
        public void AddsPresetTransactionRelatedFields()
        {
            var searchIndex = new FunctionSearchIndexDefinition<EmptyDto>(
                addPresetTransactionFields: true);

            var dto = new EmptyDto();

            var presetFields = searchIndex
                .Fields
                .Where(f => f.IsPresetSearchField()).ToArray();

            Assert.Equal(12, presetFields.Length);

            var tx = CreateDummyTransaction();

            CheckField(tx, dto, searchIndex, PresetSearchFieldName.tx_uid, 
                expectedType: typeof(string), expectedValue: "1_1", 
                isKey: true, isSortable: true, isSearchable: true);

            CheckField(tx, dto, searchIndex, PresetSearchFieldName.tx_hash, 
                expectedType: typeof(string), expectedValue: tx.TransactionHash, 
                isSortable: true, isSearchable: true, isFilterable: true);

            CheckField(tx, dto, searchIndex, PresetSearchFieldName.tx_index, 
                expectedType: typeof(HexBigInteger), expectedValue: tx.TransactionIndex, 
                isSortable: true);

            CheckField(tx, dto, searchIndex, PresetSearchFieldName.tx_block_hash, 
                expectedType: typeof(string), expectedValue: tx.BlockHash);

            CheckField(tx, dto, searchIndex, PresetSearchFieldName.tx_block_number, 
                expectedType: typeof(HexBigInteger), expectedValue: tx.BlockNumber, 
                isSortable: true, isSearchable: true, isFilterable: true, isFacetable: true);

            CheckField(tx, dto, searchIndex, PresetSearchFieldName.tx_value, 
                expectedType: typeof(HexBigInteger), expectedValue: tx.Value, 
                isSortable: true);

            CheckField(tx, dto, searchIndex, PresetSearchFieldName.tx_from, 
                expectedType: typeof(string), expectedValue: tx.From, 
                isSortable: true, isSearchable: true, isFilterable: true, isFacetable: true);

            CheckField(tx, dto, searchIndex, PresetSearchFieldName.tx_to, 
                expectedType: typeof(string), expectedValue: tx.To, 
                isSortable: true, isSearchable: true, isFilterable: true, isFacetable: true);

            CheckField(tx, dto, searchIndex, PresetSearchFieldName.tx_gas, 
                expectedType: typeof(HexBigInteger), expectedValue: tx.Gas, 
                isSortable: true);

            CheckField(tx, dto, searchIndex, PresetSearchFieldName.tx_gas_price, 
                expectedType: typeof(HexBigInteger), expectedValue: tx.GasPrice, 
                isSortable: true);

            CheckField(tx, dto, searchIndex, PresetSearchFieldName.tx_input, 
                expectedType: typeof(string), expectedValue: tx.Input);

            CheckField(tx, dto, searchIndex, PresetSearchFieldName.tx_nonce, 
                expectedType: typeof(HexBigInteger), expectedValue: tx.Nonce, 
                isSortable: true, isSearchable: true, isFilterable: true);
        }

        private void CheckField(Transaction tx,
            object dto,
            SearchIndexDefinition searchIndex,
            PresetSearchFieldName fieldName,
            Type expectedType,
            object expectedValue,
            bool isKey = false,
            bool isSortable = false,
            bool isSearchable = false,
            bool ignore = false,
            bool isFilterable = false,
            bool isFacetable = false)
        {
            var field = searchIndex.Field(fieldName);
            string property = "Null Check";
            try
            {
                Assert.NotNull(field);
                property = "DataType";
                Assert.Equal(expectedType, field.DataType);
                property = "TxValueCallback";
                Assert.NotNull(field.TxValueCallback);
                property = "IsKey";
                Assert.Equal(isKey, field.IsKey);
                property = "IsSortable";
                Assert.Equal(isSortable, field.IsSortable);
                property = "IsSearchable";
                Assert.Equal(isSearchable, field.IsSearchable);
                property = "Ignore";
                Assert.Equal(ignore, field.Ignore);
                property = "IsFilterable";
                Assert.Equal(isFilterable, field.IsFilterable);
                property = "IsFacetable";
                Assert.Equal(isFacetable, field.IsFacetable);
                property = "GetValue";
                Assert.Equal(expectedValue, field.GetValue(dto, tx));
            }
            catch (XunitException ex)
            {
                throw new Exception($"Field expectation failure: '{field.Name}':'{property}'. {ex.Message}", ex);
            }
        }

        private static Transaction CreateDummyTransaction()
        {
            return new Transaction
            {
                Value = new HexBigInteger(1000),
                Gas = new HexBigInteger(100),
                BlockNumber = new HexBigInteger(1),
                GasPrice = new HexBigInteger(50),
                BlockHash = "0xeda749446d157c1d7482c93b6dc14ffc9612b647273c009c08eac70598fc507d",
                From = "0xe6de16a66e5cd7270cc36a851818bc092884fe64",
                Input = "0x3f97cddc0000000000000000000000000000000000000000000000000000000000000002000000000000000000000000000000000000000000000000000000000000004000000000000000000000000000000000000000000000000000000000000000085368686868686868000000000000000000000000000000000000000000000000",
                Nonce = new HexBigInteger(4),
                To = "0xe6de16a66e5cd7270cc36a851818bc092884fe64",
                TransactionHash = "0xcb00b69d2594a3583309f332ada97d0df48bae00170e36a4f7bbdad7783fc7e05",
                TransactionIndex = new HexBigInteger(1)
            };
        }

        [Fact]
        public void UserDefinedPrimaryKeysOverridePresets()
        {
            var searchIndex = new FunctionSearchIndexDefinition<DepositDto>(
                addPresetTransactionFields:true);

            var dto = new DepositDto();

            var uidField = searchIndex.Field(PresetSearchFieldName.tx_uid);
            Assert.False(uidField.IsKey);

            Assert.Equal(nameof(dto.Id), searchIndex.KeyField().Name);

        }
    }
}
