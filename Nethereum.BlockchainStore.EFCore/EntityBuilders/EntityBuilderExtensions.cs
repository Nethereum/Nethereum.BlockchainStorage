using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Nethereum.BlockchainStore.EFCore.EntityBuilders
{
    public static class EntityBuilderExtensions
    {
        public const int AddressLength = 43;
        public const int HashLength = 67;
        public const int BigIntegerLength = 100;

        public static string ColumnTypeForUnlimitedText = "nvarchar(max)";

        public static PropertyBuilder<TProperty> IsUnlimitedText<TProperty>(
            this PropertyBuilder<TProperty> propertyBuilder)
        {
            return propertyBuilder.HasColumnType(ColumnTypeForUnlimitedText);
        }

        public static PropertyBuilder<TProperty> IsAddress<TProperty>(
            this PropertyBuilder<TProperty> propertyBuilder)
        {
            return propertyBuilder.HasMaxLength(AddressLength);
        }

        public static PropertyBuilder<TProperty> IsHash<TProperty>(
            this PropertyBuilder<TProperty> propertyBuilder)
        {
            return propertyBuilder.HasMaxLength(HashLength);
        }

        public static PropertyBuilder<TProperty> IsBigInteger<TProperty>(
            this PropertyBuilder<TProperty> propertyBuilder)
        {
            return propertyBuilder.HasMaxLength(BigIntegerLength);
        }
    }
}