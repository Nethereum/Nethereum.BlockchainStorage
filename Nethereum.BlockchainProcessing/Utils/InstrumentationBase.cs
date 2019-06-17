using Common.Logging;

namespace Nethereum.BlockchainProcessing.Common.Utils
{
    public abstract class InstrumentationBase
    {
        protected InstrumentationBase(ILog logger)
        {
            Logger = logger;
        }

        public virtual bool IsInfoEnabled => Logger?.IsInfoEnabled ?? false;

        public virtual bool IsTraceEnabled => Logger?.IsTraceEnabled ?? false;

        public virtual bool IsWarnEnabled => Logger?.IsWarnEnabled ?? false;

        public virtual bool IsErrorEnabled => Logger?.IsErrorEnabled ?? false;

        public ILog Logger { get; }
    }
}
