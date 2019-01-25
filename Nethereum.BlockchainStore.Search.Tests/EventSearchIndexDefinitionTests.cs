using System.Numerics;
using System.Text;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
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
        }

        [Fact]
        public void BuildsExpectedIndexForCodeGeneratedEventDto()
        {
            var searchIndex = new EventSearchIndexDefinition<DepositedEventDTO>();

            var depositedEventDto = new DepositedEventDTO
            {
                NewBalance = new HexBigInteger("100"),
                Value = new HexBigInteger("10"),
                Sender = "adsfadsfasdfasdf"
            };

            Assert.Equal("Deposited", searchIndex.IndexName);
            Assert.Equal(12, searchIndex.Fields.Length);

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
        }

        [Searchable(Name = "IndexA")]
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
            var valueAsBytes = (byte[]) searchIndex.Field("Sender").EventValue(dto);
            var valueAsString = Encoding.ASCII.GetString(valueAsBytes);
            Assert.Equal(sender, valueAsString);
        }
    }
}
