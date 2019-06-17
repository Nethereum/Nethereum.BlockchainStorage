using Common.Logging;
using System;
using System.Collections.Concurrent;

namespace Nethereum.Logging
{
    public class NestedVariablesContext : INestedVariablesContext
    {
        ConcurrentStack<string> _stack = new ConcurrentStack<string>();

        public bool HasItems => !_stack.IsEmpty;

        public void Clear() => _stack.Clear();

        public string Pop()
        {
            if(_stack.TryPop(out string _msg))
            {
                return _msg;
            }
            return string.Empty;
        }

        public IDisposable Push(string text)
        {
            _stack.Push(text);
            return new Disposable();
        }

        private class Disposable : IDisposable
        {
            public void Dispose() { }
        }
    }


}
