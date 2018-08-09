using System.ComponentModel.DataAnnotations;

namespace Nethereum.BlockchainStore.SqlServer.Entities
{
    public class AddressTransaction: TransactionBase
    {
        public string Address { get; set; }
    }
}