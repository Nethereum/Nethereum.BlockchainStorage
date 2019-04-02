using Microsoft.Azure.Search.Common;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
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
        public class DepositedEventDTO : DepositedEventDTOBase { }

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

            [SearchField]
            public List<string> Tags { get; set; }

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
            var searchIndex = new EventIndexDefinition<DepositedEventDTO>();

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
                    },
                    Tags = new List<string>{"A", "B"}
                }
            };

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

            var eventLog = new EventLog<DepositedEventDTO>(depositedEventDto, filterLog);

            Assert.Equal("Deposited", searchIndex.IndexName);
            Assert.Equal(16, searchIndex.Fields.Length);

            searchIndex
                .Assertions(eventLog)
                .HasField(nameof(depositedEventDto.Sender),
                    f => f.IsString(),
                    f => f.HasFlags(isSearchable: true, isFacetable: true, isFilterable: true, isSortable: true),
                    f => f.ReturnsValue(depositedEventDto.Sender))
                .HasField(nameof(depositedEventDto.Value),
                    f => f.DataType<BigInteger>(),
                    f => f.HasFlags(),
                    f => f.ReturnsValue(depositedEventDto.Value))
                .HasField(nameof(depositedEventDto.NewBalance),
                    f => f.DataType<BigInteger>(),
                    f => f.HasFlags(),
                    f => f.ReturnsValue(depositedEventDto.NewBalance))
                .HasField("Detail.Currency",
                    f => f.IsString(),
                    f => f.HasFlags(isSearchable: true, isFilterable: true, isFacetable: true),
                    f => f.ReturnsValue(depositedEventDto.Detail.Currency))
                .HasField("Detail.Categories.Name",
                    f => f.IsString(),
                    f => f.ReturnsValue<string[]>(categoryNameArray =>
                    {
                        var x = depositedEventDto.Detail.Categories.Select(cat => cat.Name).ToCommaSeparatedString();
                        var y = categoryNameArray.ToCommaSeparatedString();
                        return (x, y);
                    }))
                .HasField("Detail.Tags",
                    f => f.DataType<List<string>>(),
                    f => f.HasFlags(isCollection: true),
                    f => f.ReturnsValue<string[]>(tags =>
                    {
                        var x = depositedEventDto.Detail.Tags.ToCommaSeparatedString();
                        var y = tags.ToCommaSeparatedString();
                        return (x, y);
                    }))
                .HasField(PresetSearchFieldName.log_key,
                    f => f.IsString(),
                    f => f.HasFlags(isKey: true, isSortable: true),
                    f => f.ReturnsValue("101_3_9"))
                .HasField(PresetSearchFieldName.log_removed,
                    f => f.DataType<bool>(),
                    f => f.HasFlags(isFilterable: true, isSortable: true),
                    f => f.ReturnsValue(filterLog.Removed))
                .HasField(PresetSearchFieldName.log_type,
                    f => f.IsString(),
                    f => f.HasFlags(isFilterable: true, isSortable: true),
                    f => f.ReturnsValue(filterLog.Type))
                .HasField(PresetSearchFieldName.log_log_index,
                    f => f.IsHexBigInteger(),
                    f => f.HasFlags(),
                    f => f.ReturnsValue(filterLog.LogIndex))
                .HasField(PresetSearchFieldName.log_transaction_hash,
                    f => f.IsString(),
                    f => f.HasFlags(isSearchable: true),
                    f => f.ReturnsValue(filterLog.TransactionHash))
                .HasField(PresetSearchFieldName.log_transaction_index,
                    f => f.IsHexBigInteger(),
                    f => f.HasFlags(),
                    f => f.ReturnsValue(filterLog.TransactionIndex))
                .HasField(PresetSearchFieldName.log_block_hash,
                    f => f.IsString(),
                    f => f.HasFlags(),
                    f => f.ReturnsValue(filterLog.BlockHash))
                .HasField(PresetSearchFieldName.log_block_number,
                    f => f.IsHexBigInteger(),
                    f => f.HasFlags(isFilterable: true, isSearchable: true, isSortable: true),
                    f => f.ReturnsValue(filterLog.BlockNumber))
                .HasField(PresetSearchFieldName.log_address,
                    f => f.IsString(),
                    f => f.HasFlags(isFilterable: true, isSearchable: true, isSortable: true),
                    f => f.ReturnsValue(filterLog.Address));
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
            var searchIndex = new EventIndexDefinition<CustomEventDtoA>();

            var customEventDto = new CustomEventDtoA() {Category = "CatA"};

            searchIndex
                .Assertions(new EventLog<CustomEventDtoA>(customEventDto, null))
                .HasField(nameof(customEventDto.Category), 
                    f => f.IsString(),
                    f => f.HasFlags(isFacetable: true, isSearchable: true, isFilterable: true, isSortable: true),
                    f => f.ReturnsValue(customEventDto.Category));
        }

        [Fact]
        public void IndexNameCanBeOverridden()
        {
            var searchIndex = new EventIndexDefinition<CustomEventDtoA>();
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
            var searchIndex = new EventIndexDefinition<CustomEventDtoB>();
            var customEventDto = new CustomEventDtoB() {Sender = "Charles"};
            var eventLog = new EventLog<CustomEventDtoB>(customEventDto, null);

            searchIndex.Assertions(eventLog)
                .HasField("SenderAddress")
                .HasFlags(isSearchable: true, isSortable: true)
                .ReturnsValue(customEventDto.Sender);
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
            var searchIndex = new EventIndexDefinition<CustomEventDtoC>();
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
            var searchIndex = new EventIndexDefinition<CustomEventDtoD>();
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
            var searchIndex = new EventIndexDefinition<CustomEventDtoE>();
            Assert.NotNull(searchIndex.Field("Sender"));

            const string sender = "Test";
            var dto = new CustomEventDtoE {Sender = Encoding.ASCII.GetBytes(sender)};
            var valueAsBytes = searchIndex.Field("Sender").GetValue(dto) as byte[]; 
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
            var searchIndex = new EventIndexDefinition<DtoWithoutParameterAttributes>(addPresetEventLogFields: false);

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

            var eventLog = new EventLog<DtoWithoutParameterAttributes>(dto, null);

            searchIndex
                .Assertions(eventLog)
                .HasField(nameof(dto.Name), 
                    f => f.ReturnsValue(dto.Name))
                .HasField(nameof(dto.Values), 
                    f => f.IsCollection(),
                    f => f.ReturnsValue<int[]>(actualValues => (dto.Values.ToCommaSeparatedString(), actualValues.ToCommaSeparatedString())))
                .HasField($"{nameof(dto.Metadata)}.{nameof(dto.Metadata.Id)}", 
                    f => f.HasFlags(isSearchable: true), 
                    f => f.ReturnsValue(dto.Metadata.Id))
                .HasField($"{nameof(dto.Metadata)}.{nameof(dto.Metadata.Description)}", 
                    f => f.HasFlags().ReturnsValue(dto.Metadata.Description))
                .HasField($"{nameof(dto.Tags)}.Value", 
                    f => f.HasFlags(isCollection: true, isSearchable: true, isFilterable: true), 
                    f => f.ReturnsValue<string[]>(actualTags => (dto.Tags.Select(item => item.Value).ToCommaSeparatedString(), actualTags.ToCommaSeparatedString()))
                );
        }

        [Event("Transfer")]
        public class TransferEvent_Custom
        {
            [Parameter("address", "_from", 1, true)]
            public string From {get; set;}

            [Parameter("address", "_to", 2, true)]
            public string To {get; set;}

            [Parameter("uint256", "_value", 3, true)]
            public BigInteger Value {get; set;}

            public TransferMetadata Metadata { get; set; } = new TransferMetadata();
        }

        public class TransferMetadata
        {
            [SearchField]
            public static string CurrentChainUrl { get;set; }
        }

        [Fact]
        public void StaticPropertiesOnDtoAreIgnored()
        {
            var searchIndex = new EventIndexDefinition<TransferEvent_Custom>(addPresetEventLogFields:false);
            Assert.Null(searchIndex.Field("Metadata.CurrentChainUrl"));
        }
    }
}
