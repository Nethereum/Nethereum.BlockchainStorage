using Nethereum.BlockchainProcessing.BlockStorage.Entities;
using System.Data.Entity.ModelConfiguration;

namespace Nethereum.BlockchainStore.EF.EntityBuilders
{
    public class BlockEntityBuilder: EntityTypeConfiguration<Block>
    {
        public BlockEntityBuilder()
        {
            ToTable("Blocks");
            HasKey(b => b.RowIndex);

            Property(b => b.BlockNumber).IsBigInteger().IsRequired();
            Property(b => b.Hash).IsHash().IsRequired();
            Property(b => b.ParentHash).IsHash().IsRequired();
            Property(b => b.Miner).IsAddress();
            HasIndex(b => new {b.BlockNumber, b.Hash}).HasName("IX_Block_BlockNumber_Hash").IsUnique();                

            Property(b => b.Difficulty).IsBigInteger();
            Property(b => b.TotalDifficulty).IsBigInteger();
            Property(b => b.Size).IsBigInteger();
            Property(b => b.GasLimit).IsBigInteger();
            Property(b => b.GasUsed).IsBigInteger();
            Property(b => b.Timestamp).IsBigInteger();
            Property(b => b.Nonce).IsBigInteger();
            Property(b => b.BaseFeePerGas).IsBigInteger();
        }
    }
}