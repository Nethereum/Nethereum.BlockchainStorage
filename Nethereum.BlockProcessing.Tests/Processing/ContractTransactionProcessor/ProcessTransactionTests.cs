using System;
using System.Threading.Tasks;
using Nethereum.BlockchainStore.Tests.Processing.ContractTransactionProcessorTests.Scenarios;
using Nethereum.Hex.HexTypes;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Nethereum.BlockProcessing.Tests
{

    public class ProcessTransactionTests : ProcessTransactionScenario, IDisposable
    {
        readonly JObject _vmStack = new JObject();
        string _error = "oops";
        bool _hasError = true;
        bool _hasStackTrace = true;

        protected void SetUpMocks()
        {
            MockGetVmStack(_vmStack);
            MockGetErrorFromVmStack(_vmStack, _error);
            MockHandleVmStack(_vmStack);
            MockHandleTransaction(_hasError, _hasStackTrace, _error);
        }

        [Fact]
        public async Task Invokes_Handlers_For_VmStack_Transactions_And_Logs()
        {
            SetUpMocks();
            InitProcessor();

            //execute
            await _processor.ProcessTransactionAsync(_transaction, _receipt, _blockTimestamp);

            //assert
            EnsureHandleVmStackWasInvoked();
            EnsureHandleTransactionWasInvoked();
        }

        [Fact]
        public async Task When_Log_Does_Not_Match_Filter_Handle_Log_Is_Not_Invoked()
        {
            SetUpMocks();
            AddFilterWhichDoesNotMatchAnyLog();
            InitProcessor();

            //execute
            await _processor.ProcessTransactionAsync(_transaction, _receipt, _blockTimestamp);

            //assert
            EnsureHandleTxLogWasNotInvoked();
        }

        [Fact]
        public async Task When_Receipt_Status_Does_Not_Equal_One_An_Error_Has_Occurred()
        {
            _error = "";
            _hasError = true;
            _hasStackTrace = false;

            _receipt.Status = new HexBigInteger(0);

            MockExceptionFromGetTransactionVmStack();
            MockHandleTransaction(_hasError, _hasStackTrace, _error);

            InitProcessor();

            //execute
            await _processor.ProcessTransactionAsync(_transaction, _receipt, _blockTimestamp);

            //assert
            EnsureHandleTransactionWasInvoked();
        }

        [Fact]
        public async Task When_Stack_Trace_Contains_An_Error_It_Is_Treated_As_An_Error()
        {
            _error = "Bad Error";
            _hasError = true;
            _hasStackTrace = true;

            MockGetVmStack(_vmStack);
            MockGetErrorFromVmStack(_vmStack, _error);
            MockHandleVmStack(_vmStack);
            MockHandleTransaction(_hasError, _hasStackTrace, _error);

            InitProcessor();
            //execute
            await _processor.ProcessTransactionAsync(_transaction, _receipt, _blockTimestamp);

            //assert
            EnsureHandleVmStackWasInvoked();
            EnsureHandleTransactionWasInvoked();
        }

        [Fact]
        public async Task When_Vm_Processing_Is_Not_Enabled_No_Vm_Stack_Processing_Occurs()
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
            EnsureHandleTransactionWasInvoked();
        }

        public void Dispose()
        {
            ClearVmStackMocks();
        }
    }

}
