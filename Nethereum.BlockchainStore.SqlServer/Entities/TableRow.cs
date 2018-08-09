namespace Nethereum.BlockchainStore.SqlServer.Entities
{
    public class TableRow
    {
        public int RowIndex { get; set; }

        public bool IsNew() => RowIndex == 0;
    }
}