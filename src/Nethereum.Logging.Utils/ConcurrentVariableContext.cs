using System.Collections.Concurrent;
using System.Collections.Generic;
using Common.Logging;

namespace Nethereum.Microsoft.Logging.Utils
{
    public class ConcurrentVariableContext : IVariablesContext
    {
        readonly ConcurrentDictionary<string, object> _variables = new ConcurrentDictionary<string, object>();

        public IReadOnlyList<KeyValuePair<string, object>> GetAll() => _variables.ToArray();

        public void Set(string key, object value) => this._variables[key] = value;

        public object Get(string key)
        {
            object value;
            return _variables.TryGetValue(key, out value) ? value : null;
        }

        public bool Contains(string key) => this._variables.ContainsKey(key);

        public void Remove(string key)
        {
            object dummy;
            _variables.TryRemove(key, out dummy);
        }

        public void Clear() => _variables.Clear();
    }
}
