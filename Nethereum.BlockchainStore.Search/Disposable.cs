using System.Collections.Generic;
using System.Threading.Tasks;

namespace System
{
    public class DisposableContext : IDisposable
    {
        readonly Stack<IDisposable> _disposables = new Stack<IDisposable>();

        public T Add<T>(T disposable) where T : IDisposable
        {
            _disposables.Push(disposable);
            return disposable;
        }

        public T Add<T>(Func<T> constructor) where T : IDisposable
        {
            T obj = constructor();
            _disposables.Push(obj);
            return obj;
        }

        public async Task<T> Add<T>(Func<Task<T>> constructor) where T : IDisposable
        {
            var task = await constructor();
            _disposables.Push(task);
            return task;
        }

        public void Dispose()
        {
            while (_disposables.Count > 0)
            {
                _disposables.Pop().Dispose();
            }
        }
    }
}
