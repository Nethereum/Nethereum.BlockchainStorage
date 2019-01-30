using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using Xunit;

namespace Nethereum.BlockchainStore.Search.Tests
{
    public class EventSearchIndexDefinitionTests
    {
        public partial class DepositedEventDTO : DepositedEventDTOBase { }

        [Event("Deposited")]
        public class DepositedEventDTOBase : IEventDTO
        {
            [Parameter("address", "_sender", 1, true )]
            public virtual string Sender { get; set; }

            [Parameter("uint256", "_value", 2, false )]
            public virtual BigInteger Value { get; set; }

            [Parameter("uint256", "_newBalance", 3, false)]
            public virtual BigInteger NewBalance { get; set; }

            //a struct
            [Parameter("tuple", "_detail", 3, false)]
            public virtual DepositDetailDTO Detail { get; set; }
        }

        //a class to represent a struct
        public class DepositDetailDTO
        {
            [Parameter("uint256", "_timestamp", 1, false )]
            public long Timestamp { get; set; }

            [SearchField(IsSearchable = true, IsFilterable = true, IsFacetable = true)]
            [Parameter("string", "_currency", 2, false )]
            public string Currency { get; set; }

            //an array of structs
            [Parameter("tuple[2]", "_categories", 3, false )]
            public List<CategoryDTO> Categories { get; set; }

 
        }

        //another struct - expected to be returned in an array
        public class CategoryDTO
        {
            [Parameter("string", "_name", 1, false)]
            public string Name { get; set; }
        }

        [Fact]
        public void BuildsExpectedIndexForCodeGeneratedEventDto()
        {
            var searchIndex = new EventSearchIndexDefinition<DepositedEventDTO>();

            var depositedEventDto = new DepositedEventDTO
            {
                NewBalance = new HexBigInteger("100"),
                Value = new HexBigInteger("10"),
                Sender = "adsfadsfasdfasdf",
                Detail = new DepositDetailDTO
                {
                    Currency = "GBP",
                    Timestamp = DateTimeOffset.UnixEpoch.ToUnixTimeSeconds(),
                    Categories = new List<CategoryDTO>
                    {
                        new CategoryDTO{Name = "Dodgy"}, 
                        new CategoryDTO{Name = "International"}
                    }
                }
            };

            Assert.Equal("Deposited", searchIndex.IndexName);
            Assert.Equal(15, searchIndex.Fields.Length);

            var senderField = searchIndex.Field(nameof(depositedEventDto.Sender));
            Assert.NotNull(senderField);
            Assert.Equal(typeof(string), senderField.DataType);
            Assert.False(senderField.IsKey);
            Assert.False(senderField.Ignore);
            Assert.True(senderField.IsFacetable);
            Assert.True(senderField.IsFilterable);
            Assert.True(senderField.IsSearchable);
            Assert.True(senderField.IsSortable);
            Assert.Equal(depositedEventDto.Sender, senderField.EventValue(depositedEventDto));

            var valueField = searchIndex.Field(nameof(depositedEventDto.Value));
            Assert.NotNull(valueField);
            Assert.Equal(typeof(BigInteger), valueField.DataType);
            Assert.False(valueField.IsKey);
            Assert.False(valueField.Ignore);
            Assert.False(valueField.IsSearchable);
            Assert.False(valueField.IsFilterable);
            Assert.False(valueField.IsFacetable);
            Assert.False(valueField.IsSortable);
            Assert.Equal(depositedEventDto.Value, valueField.EventValue(depositedEventDto));

            var newBalanceField = searchIndex.Field(nameof(depositedEventDto.NewBalance));
            Assert.NotNull(newBalanceField);
            Assert.Equal(typeof(BigInteger), newBalanceField.DataType);
            Assert.False(newBalanceField.IsKey);
            Assert.False(newBalanceField.Ignore);
            Assert.False(newBalanceField.IsSearchable);
            Assert.False(newBalanceField.IsFilterable);
            Assert.False(newBalanceField.IsFacetable);
            Assert.False(newBalanceField.IsSortable);
            Assert.Equal(depositedEventDto.NewBalance, newBalanceField.EventValue(depositedEventDto));

            var log_key = searchIndex.Field(PresetSearchFieldName.log_key);
            var log_removed = searchIndex.Field(PresetSearchFieldName.log_removed);
            var log_type = searchIndex.Field(PresetSearchFieldName.log_type);
            var log_logIndex = searchIndex.Field(PresetSearchFieldName.log_log_index);
            var log_transactionIndex = searchIndex.Field(PresetSearchFieldName.log_transaction_index);
            var log_transactionHash = searchIndex.Field(PresetSearchFieldName.log_transaction_hash);
            var log_blockHash = searchIndex.Field(PresetSearchFieldName.log_block_hash);
            var log_blockNumber = searchIndex.Field(PresetSearchFieldName.log_block_number);
            var log_address = searchIndex.Field(PresetSearchFieldName.log_address);

            var filterLog = new FilterLog
            {
                Type = "type",
                Address = "address",
                BlockHash = "block_hash",
                BlockNumber = new HexBigInteger(101),
                TransactionHash = "transaction_hash",
                TransactionIndex = new HexBigInteger(3),
                LogIndex = new HexBigInteger(9),
                Removed = false
            };

            Assert.NotNull(log_key);
            Assert.Equal(typeof(string), log_key.DataType);
            Assert.True(log_key.IsKey);
            Assert.True(log_key.IsSortable);
            Assert.False(log_key.Ignore);
            Assert.False( log_key.IsSearchable);
            Assert.False(log_key.IsFacetable);
            Assert.False(log_key.IsFilterable);
            Assert.Equal("101_3_9", log_key.LogValueCallback(filterLog));

            Assert.NotNull(log_removed);
            Assert.Equal(typeof(bool), log_removed.DataType);
            Assert.False(log_removed.Ignore);
            Assert.False(log_removed.IsKey);
            Assert.False(log_removed.IsFacetable);
            Assert.True(log_removed.IsFilterable);
            Assert.False(log_removed.IsSearchable);
            Assert.True(log_removed.IsSortable);
            Assert.Equal(filterLog.Removed, log_removed.LogValueCallback(filterLog));
            
            Assert.NotNull(log_type);
            Assert.Equal(typeof(string), log_type.DataType);
            Assert.False(log_type.Ignore);
            Assert.False(log_type.IsKey);
            Assert.False(log_type.IsFacetable);
            Assert.True(log_type.IsFilterable);
            Assert.False(log_type.IsSearchable);
            Assert.True(log_type.IsSortable);
            Assert.Equal(filterLog.Type, log_type.LogValueCallback(filterLog));

            Assert.NotNull(log_logIndex);
            Assert.Equal(typeof(HexBigInteger), log_logIndex.DataType);
            Assert.False(log_logIndex.Ignore);
            Assert.False(log_logIndex.IsKey);
            Assert.False(log_logIndex.IsFacetable);
            Assert.False(log_logIndex.IsFilterable);
            Assert.False(log_logIndex.IsSearchable);
            Assert.False(log_logIndex.IsSortable);
            Assert.Equal(filterLog.LogIndex, log_logIndex.LogValueCallback(filterLog));

            Assert.NotNull(log_transactionHash);
            Assert.Equal(typeof(string), log_transactionHash.DataType);
            Assert.False(log_transactionHash.Ignore);
            Assert.False(log_transactionHash.IsKey);
            Assert.False(log_transactionHash.IsFacetable);
            Assert.False(log_transactionHash.IsFilterable);
            Assert.True(log_transactionHash.IsSearchable);
            Assert.False(log_transactionHash.IsSortable);
            Assert.Equal(filterLog.TransactionHash, log_transactionHash.LogValueCallback(filterLog));

            Assert.NotNull(log_transactionIndex);
            Assert.Equal(typeof(HexBigInteger), log_transactionIndex.DataType);
            Assert.False(log_transactionIndex.Ignore);
            Assert.False(log_transactionIndex.IsKey);
            Assert.False(log_transactionIndex.IsFacetable);
            Assert.False(log_transactionIndex.IsFilterable);
            Assert.False(log_transactionIndex.IsSearchable);
            Assert.False(log_transactionIndex.IsSortable);
            Assert.Equal(filterLog.TransactionIndex, log_transactionIndex.LogValueCallback(filterLog));

            Assert.NotNull(log_blockHash);
            Assert.Equal(typeof(string), log_blockHash.DataType);
            Assert.False(log_blockHash.Ignore);
            Assert.False(log_blockHash.IsKey);
            Assert.False(log_blockHash.IsFacetable);
            Assert.False(log_blockHash.IsFilterable);
            Assert.False(log_blockHash.IsSearchable);
            Assert.False(log_blockHash.IsSortable);
            Assert.Equal(filterLog.BlockHash, log_blockHash.LogValueCallback(filterLog));

            Assert.NotNull(log_blockNumber);
            Assert.Equal(typeof(HexBigInteger), log_blockNumber.DataType);
            Assert.False(log_blockNumber.Ignore);
            Assert.False(log_blockNumber.IsKey);
            Assert.False(log_blockNumber.IsFacetable);
            Assert.True(log_blockNumber.IsFilterable);
            Assert.True(log_blockNumber.IsSearchable);
            Assert.True(log_blockNumber.IsSortable);
            Assert.Equal(filterLog.BlockNumber, log_blockNumber.LogValueCallback(filterLog));

            Assert.NotNull(log_address);
            Assert.Equal(typeof(string), log_address.DataType);
            Assert.False(log_address.Ignore);
            Assert.False(log_address.IsKey);
            Assert.False(log_address.IsFacetable);
            Assert.True(log_address.IsFilterable);
            Assert.True(log_address.IsSearchable);
            Assert.True(log_address.IsSortable);
            Assert.Equal(filterLog.Address, log_address.LogValueCallback(filterLog));

            var currencyField = searchIndex.Field("Detail.Currency");
            Assert.NotNull(currencyField);
            Assert.True(currencyField.IsSearchable);
            Assert.True(currencyField.IsFilterable);
            Assert.True(currencyField.IsFacetable);
            Assert.Equal(depositedEventDto.Detail.Currency, currencyField.EventValue(depositedEventDto));


            var categoryNameField = searchIndex.Field("Detail.Categories.Name");
            Assert.NotNull(categoryNameField);
            Assert.True(categoryNameField.IsCollection);
            var categoryNames = categoryNameField.EventValue(depositedEventDto) as string[];
            Assert.NotNull(categoryNames);
            Assert.Equal(depositedEventDto.Detail.Categories.Count, categoryNames.Length);
            Assert.Equal(depositedEventDto.Detail.Categories[0].Name, (string)categoryNames[0]);
            Assert.Equal(depositedEventDto.Detail.Categories[1].Name, (string)categoryNames[1]);
        }

        [SearchIndex(Name = "IndexA")]
        public partial class CustomEventDtoA : DepositedEventDTOBase
        {
            [SearchField("Category", 
                IsFacetable = true, IsSearchable = true, IsFilterable = true, IsSortable = true)]
            public virtual string Category { get;set; }
        }

        [Fact]
        public void IndexCanIncludeCustomFieldsOutsideOfTheSolidityEvent()
        {
            var searchIndex = new EventSearchIndexDefinition<CustomEventDtoA>();

            var categoryField = searchIndex.Field("Category");
            Assert.NotNull(categoryField);
            Assert.False(categoryField.Ignore);
            Assert.False(categoryField.IsKey);
            Assert.True(categoryField.IsFacetable);
            Assert.True(categoryField.IsSearchable);
            Assert.True(categoryField.IsFilterable);
            Assert.True(categoryField.IsSortable);

            var customEventDto = new CustomEventDtoA() {Category = "CatA"};

            Assert.Equal(customEventDto.Category, categoryField.EventValue(customEventDto));

        }

        [Fact]
        public void IndexNameCanBeOverridden()
        {
            var searchIndex = new EventSearchIndexDefinition<CustomEventDtoA>();
            Assert.Equal("IndexA", searchIndex.IndexName);
        }

       
        public partial class CustomEventDtoB : DepositedEventDTOBase
        {
            [SearchField("SenderAddress", 
                IsFacetable = false, IsSearchable = true, IsFilterable = false, IsSortable = true)]
            [Parameter("address", "_sender", 1, true )]
            public override string Sender { get; set; }
        }

        [Fact]
        public void EventParametersCanBeCustomIndexed()
        {
            var searchIndex = new EventSearchIndexDefinition<CustomEventDtoB>();

            Assert.Null(searchIndex.Field("Sender"));

            var senderAddressField = searchIndex.Field("SenderAddress");
            Assert.NotNull(senderAddressField);
            Assert.False(senderAddressField.Ignore);
            Assert.False(senderAddressField.IsKey);
            Assert.False(senderAddressField.IsFacetable);
            Assert.True(senderAddressField.IsSearchable);
            Assert.False(senderAddressField.IsFilterable);
            Assert.True(senderAddressField.IsSortable);

            var customEventDto = new CustomEventDtoB() {Sender = "Charles"};

            Assert.Equal(customEventDto.Sender, senderAddressField.EventValue(customEventDto));

        }

        public partial class CustomEventDtoC : DepositedEventDTOBase
        {
            [SearchField(Ignore = true)]
            [Parameter("address", "_sender", 1, true )]
            public override string Sender { get; set; }
        }

        [Fact]
        public void WillExcludeIgnoredFields()
        {
            var searchIndex = new EventSearchIndexDefinition<CustomEventDtoC>();
            Assert.Null(searchIndex.Field("Sender"));
        }

        public partial class CustomEventDtoD : DepositedEventDTOBase
        {
            [SearchField]
            [Parameter("address", "_sender", 1, true )]
            public override string Sender { get; set; }
        }

        [Fact]
        public void WillProvideDefaultPropertyName()
        {
            var searchIndex = new EventSearchIndexDefinition<CustomEventDtoD>();
            Assert.NotNull(searchIndex.Field("Sender"));
        }

        public partial class CustomEventDtoE : DepositedEventDTOBase
        {
            [Parameter("address", "_sender", 1, true )]
            public new byte[] Sender { get; set; }
        }

        [Fact]
        public void WillIndexNewProperties()
        {
            var searchIndex = new EventSearchIndexDefinition<CustomEventDtoE>();
            Assert.NotNull(searchIndex.Field("Sender"));

            const string sender = "Test";
            var dto = new CustomEventDtoE {Sender = Encoding.ASCII.GetBytes(sender)};
            var valueAsBytes = searchIndex.Field("Sender").EventValue(dto) as byte[]; 
            var valueAsString = Encoding.ASCII.GetString(valueAsBytes);
            Assert.Equal(sender, valueAsString);
        }

        [SearchIndex("CustomIndexA")]
        public class DtoWithoutParameterAttributes
        {
            [SearchField]
            public string Name { get;set; }
        
            // do not place SearchField attribute on complex object properties
            public MetadataDto Metadata { get; set; }

            // do not place SearchField attribute on arrays or lists of complex objects
            public List<TagDto> Tags { get; set; }

            [SearchField]
            public List<int> Values { get; set; } 
        }

        public class MetadataDto
        {
            [SearchField(IsSearchable = true)]
            public string Id { get; set; }

            [SearchField]
            public string Description { get; set; }
        }

        public class TagDto
        {
            [SearchField(IsSearchable = true, IsFilterable = true)]
            public string Value { get; set; }
        }

        [Fact]
        public void WillIndexDtoWithOnlySearchFieldAttributes()
        {
            var searchIndex = new EventSearchIndexDefinition<DtoWithoutParameterAttributes>(addStandardBlockchainFields: false);

            var dto = new DtoWithoutParameterAttributes
            {
                Name = "Test",
                Metadata = new MetadataDto { Id = "A1", Description = "first description"},
                Tags = new List<TagDto>
                {
                    new TagDto() {Value = "Category A"},
                    new TagDto() {Value = "Category B"}
                },
                Values = new List<int>(){1, 2, 3}
            };

            Assert.Equal("CustomIndexA", searchIndex.IndexName);

            Assert.Equal(5, searchIndex.Fields.Length);

            var nameField = searchIndex.Field(nameof(dto.Name));
            Assert.NotNull(nameField);
            Assert.Equal(dto.Name, nameField.EventValue(dto));

            var valuesField = searchIndex.Field(nameof(dto.Values));
            Assert.NotNull(valuesField);
            Assert.True(valuesField.IsCollection);
            var values = valuesField.EventValue(dto) as int[];
            Assert.NotNull(values);
            Assert.Equal(1, values[0]);
            Assert.Equal(2, values[1]);
            Assert.Equal(3, values[2]);

            var metaDataIdField = searchIndex.Field($"{nameof(dto.Metadata)}.{nameof(dto.Metadata.Id)}");
            Assert.NotNull(metaDataIdField);
            Assert.True(metaDataIdField.IsSearchable);
            Assert.Equal(dto.Metadata.Id, metaDataIdField.EventValue(dto));
           
            var metaDataDescriptionField = searchIndex.Field($"{nameof(dto.Metadata)}.{nameof(dto.Metadata.Description)}");
            Assert.NotNull(metaDataDescriptionField);
            Assert.False(metaDataDescriptionField.IsSearchable);
            Assert.Equal(dto.Metadata.Description, metaDataDescriptionField.EventValue(dto));

            var tagsValueField = searchIndex.Field($"{nameof(dto.Tags)}.Value");
            Assert.NotNull(tagsValueField);
            Assert.True(tagsValueField.IsCollection);
            Assert.True(tagsValueField.IsSearchable);
            Assert.True(tagsValueField.IsFilterable);

            var tagValues = tagsValueField.EventValue(dto) as string[];
            Assert.NotNull(tagValues);
            Assert.Equal(dto.Tags.Count, tagValues.Length);
            Assert.Equal(dto.Tags[0].Value, tagValues[0]);
            Assert.Equal(dto.Tags[1].Value, tagValues[1]);
        }
    }
}
