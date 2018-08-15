using System.Data.Entity.ModelConfiguration.Configuration;
using Nethereum.BlockchainStore.Entities;

namespace Nethereum.BlockchainStore.EF.EntityBuilders
{
    public static class EntityBuilderExtensions
    {
        public static string ColumnTypeForUnlimitedText = "nvarchar(max)";

        public static StringPropertyConfiguration IsUnlimitedText(
            this StringPropertyConfiguration propertyBuilder)
        {
            return propertyBuilder.HasColumnType(ColumnTypeForUnlimitedText);
        }

        public static StringPropertyConfiguration IsAddress(
            this StringPropertyConfiguration propertyBuilder)
        {
            return propertyBuilder.HasMaxLength(ColumnLengths.AddressLength);
        }

        public static StringPropertyConfiguration IsHash(
            this StringPropertyConfiguration propertyBuilder)
        {
            return propertyBuilder.HasMaxLength(ColumnLengths.HashLength);
        }

        public static StringPropertyConfiguration IsBigInteger(
            this StringPropertyConfiguration propertyBuilder)
        {
            return propertyBuilder.HasMaxLength(ColumnLengths.BigIntegerLength);
        }
    }
}