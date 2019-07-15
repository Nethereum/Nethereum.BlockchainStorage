using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Test.Base.RepositoryTests
{
    public interface IRepositoryTest
    {
        Task RunAsync();
    }
}
