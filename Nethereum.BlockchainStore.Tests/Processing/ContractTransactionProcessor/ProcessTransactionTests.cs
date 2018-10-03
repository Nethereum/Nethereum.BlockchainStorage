using System.Threading.Tasks;
using Nethereum.BlockchainStore.Tests.Processing.ContractTransactionProcessorTests.Scenarios;
using Nethereum.Hex.HexTypes;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Nethereum.BlockchainStore.Tests.Processing.ContractTransactionProcessorTests
{

    public class ProcessTransactionTests : ProcessTransactionScenario
    {
        JObject _vmStack = new JObject();
        string _error = "oops";
        bool _hasError = true;
        bool _hasStackTrace = true;

        protected void SetUpMocks()
        {
            MockGetVmStack(_vmStack);
            MockGetErrorFromVmStack(_vmStack, _error);
            MockVmStackUpsert(_vmStack);
            MockTransactionUpsert(_hasError, _hasStackTrace, _error);
            MockAddressTransactionUpsert(_hasError, _hasStackTrace, _error);
            MockTransactionLogUpserts();
        }

        [Fact]
        public async Task Upserts_VmStack_Transactions_Logs()
        {
            SetUpMocks();
            InitProcessor();

            //execute
            await _processor.ProcessTransactionAsync(_transaction, _receipt, _blockTimestamp);

            //assert
            VerifyVmStackUpsert();
            VerifyTransactionUpsert();
            VerifyAddressTransactionUpsert();
            VerifyTxLogUpserts();
        }

        [Fact]
        public async Task WhenLogDoesNotMatchFilter_TheLogIsNotUpserted()
        {
            SetUpMocks();
            AddFilterWhichDoesNotMatchAnyLog();
            InitProcessor();

            //execute
            await _processor.ProcessTransactionAsync(_transaction, _receipt, _blockTimestamp);

            //assert
            VerifyNoLogsHaveBeenUpserted();
        }

        [Fact]
        public async Task WhenGetVmStackThrows_And_TxGasEqualsReceiptGas_AnErrorIsAssumed()
        {
            _error = "";
            _hasError = true;
            _hasStackTrace = false;

            var gas = new HexBigInteger(10);
            _transaction.Gas = gas;
            _receipt.GasUsed = _transaction.Gas;

            MockExceptionFromGetTransactionVmStack();
            MockTransactionUpsert(_hasError, _hasStackTrace, _error);
            MockAddressTransactionUpsert(_hasError, _hasStackTrace, _error);
            MockTransactionLogUpserts();

            InitProcessor();

            //execute
            await _processor.ProcessTransactionAsync(_transaction, _receipt, _blockTimestamp);

            //assert
            VerifyTransactionUpsert();
            VerifyAddressTransactionUpsert();
            VerifyTxLogUpserts();
        }

        [Fact]
        public async Task WhenStackTraceContainsAnError_ItIsTreatedAsAnError()
        {
            _error = "Bad Error";
            _hasError = true;
            _hasStackTrace = true;

            MockGetVmStack(_vmStack);
            MockGetErrorFromVmStack(_vmStack, _error);
            MockVmStackUpsert(_vmStack);
            MockTransactionUpsert(_hasError, _hasStackTrace, _error);
            MockAddressTransactionUpsert(_hasError, _hasStackTrace, _error);
            MockTransactionLogUpserts();

            InitProcessor();
            //execute
            await _processor.ProcessTransactionAsync(_transaction, _receipt, _blockTimestamp);

            //assert
            VerifyVmStackUpsert();
            VerifyTransactionUpsert();
            VerifyAddressTransactionUpsert();
            VerifyTxLogUpserts();
        }

        [Fact]
        public async Task WhenVmProcessingIsNotEnabled_NoVmStackProcessingOccurs()
        {
            _error = "";
            _hasError = false;
            _hasStackTrace = false;

            SetUpMocks();
            InitProcessor(enableVmStackProcessing: false);

            //execute
            await _processor.ProcessTransactionAsync(_transaction, _receipt, _blockTimestamp);

            //assert
            VerifyVmStackInfoHasNotBeenProcessed();
            VerifyTransactionUpsert();
            VerifyAddressTransactionUpsert();
            VerifyTxLogUpserts();
        }
    }

}
