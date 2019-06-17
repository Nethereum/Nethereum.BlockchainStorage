using Common.Logging;
using System;
using System.Collections.Concurrent;

namespace Nethereum.Logging
{
    public class ConcurrentNestedVariablesContext : INestedVariablesContext
    {
        private readonly ConcurrentStack<string> _stack = new ConcurrentStack<string>();

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
            return Disposable.Dummy;
        }

        private class Disposable : IDisposable
        {
            public readonly static Disposable Dummy = new Disposable();
            
            public void Dispose() { }
        }
    }


}
