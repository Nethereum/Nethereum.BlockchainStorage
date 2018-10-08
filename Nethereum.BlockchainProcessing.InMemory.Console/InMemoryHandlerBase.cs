using System;

namespace Nethereum.BlockchainStore.Processing
{
    public abstract class InMemoryHandlerBase
    {
        private readonly Action<string> _logAction;

        protected virtual void Log(string message)
        {
            _logAction(message);
        }

        protected InMemoryHandlerBase(Action<string> logAction)
        {
            this._logAction = logAction;
        }
    }
}
