using Moq;
using Nethereum.Contracts.Services;
using Nethereum.RPC.Eth;
using Nethereum.RPC.Eth.Blocks;
using Nethereum.RPC.Eth.Filters;
using Nethereum.RPC.Eth.Services;
using Nethereum.RPC.Eth.Transactions;
using Nethereum.Web3;

namespace Nethereum.BlockchainProcessing.Tests.Processing
{
    public class Web3Mock
    {
        public IWeb3 Web3 => Mock.Object;

        public Mock<IWeb3> Mock = new Mock<IWeb3>();
        public Mock<IEthApiContractService> ContractServiceMock = new Mock<IEthApiContractService>();

        public Mock<IEthApiBlockService> BlocksServiceMock = new Mock<IEthApiBlockService>();

        public Mock<IEthBlockNumber> BlockNumberMock = new Mock<IEthBlockNumber>();

        public Mock<IEthGetLogs> GetLogsMock = new Mock<IEthGetLogs>();

        public Mock<IEthApiFilterService> FilterServiceMock = new Mock<IEthApiFilterService>();

        public Mock<IEthApiTransactionsService> TransactionServiceMock = new Mock<IEthApiTransactionsService>();

        public Mock<IEthGetTransactionByHash> GetTransactionByHashMock = new Mock<IEthGetTransactionByHash>();

        public Mock<IEthGetTransactionReceipt> GetTransactionReceiptMock = new Mock<IEthGetTransactionReceipt>();

        public Mock<IEthGetBlockWithTransactionsByNumber> GetBlockWithTransactionsByNumberMock = new Mock<IEthGetBlockWithTransactionsByNumber>();

        public Mock<IEthGetCode> GetCodeMock = new Mock<IEthGetCode>();

        public IEthApiContractService Eth => ContractServiceMock.Object;

        public Web3Mock()
        {
            Mock.Setup(m => m.Eth).Returns(ContractServiceMock.Object);
            ContractServiceMock.Setup(e => e.Blocks).Returns(BlocksServiceMock.Object);
            ContractServiceMock.Setup(e => e.GetCode).Returns(GetCodeMock.Object);
            BlocksServiceMock.Setup(b => b.GetBlockNumber).Returns(BlockNumberMock.Object);
            BlocksServiceMock.Setup(b => b.GetBlockWithTransactionsByNumber).Returns(GetBlockWithTransactionsByNumberMock.Object);
            ContractServiceMock.Setup(e => e.Filters).Returns(FilterServiceMock.Object);
            FilterServiceMock.Setup(f => f.GetLogs).Returns(GetLogsMock.Object);
            ContractServiceMock.Setup(c => c.Transactions).Returns(TransactionServiceMock.Object);
            TransactionServiceMock.Setup(t => t.GetTransactionByHash).Returns(GetTransactionByHashMock.Object);
            TransactionServiceMock.Setup(t => t.GetTransactionReceipt).Returns(GetTransactionReceiptMock.Object);
        }

    }
}
