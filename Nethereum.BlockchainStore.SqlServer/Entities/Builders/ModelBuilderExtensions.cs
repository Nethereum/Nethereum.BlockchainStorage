using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Nethereum.BlockchainStore.SqlServer
{
    public static class ModelBuilderExtensions
    {
        public const int AddressLength = 43;
        public const int HashLength = 67;
        public const int BigIntegerLength = 100;

        public static PropertyBuilder<TProperty> IsNVarcharMax<TProperty>(
            this PropertyBuilder<TProperty> propertyBuilder)
        {
            return propertyBuilder.HasColumnType("nvarchar(max)");
        }

        public static PropertyBuilder<TProperty> IsAddress<TProperty>(
            this PropertyBuilder<TProperty> propertyBuilder)
        {
            return propertyBuilder.HasMaxLength(HashLength);
        }

        public static PropertyBuilder<TProperty> IsHash<TProperty>(
            this PropertyBuilder<TProperty> propertyBuilder)
        {
            return propertyBuilder.HasMaxLength(HashLength);
        }

        public static PropertyBuilder<TProperty> IsBigInteger<TProperty>(
            this PropertyBuilder<TProperty> propertyBuilder)
        {
            return propertyBuilder.HasMaxLength(HashLength);
        }
    }
}