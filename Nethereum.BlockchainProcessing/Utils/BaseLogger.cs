using Common.Logging;

namespace Nethereum.BlockchainProcessing.Common.Utils
{
    public abstract class BaseLogger
    {
        protected BaseLogger(ILog logger)
        {
            Logger = logger;
        }

        public bool IsInfoEnabled => Logger?.IsInfoEnabled ?? false;

        public bool IsTraceEnabled => Logger?.IsTraceEnabled ?? false;

        public bool IsWarnEnabled => Logger?.IsWarnEnabled ?? false;

        public bool IsErrorEnabled => Logger?.IsErrorEnabled ?? false;

        public ILog Logger { get; }
    }
}
