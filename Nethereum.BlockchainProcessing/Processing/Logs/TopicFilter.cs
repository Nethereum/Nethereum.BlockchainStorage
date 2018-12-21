using System.Collections.Generic;
using System.Reflection;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public class TopicFilter
    {
        public static readonly TopicFilter Empty = new TopicFilter(null, null);

        private List<object> _values;

        public TopicFilter(PropertyInfo eventDtoProperty, ParameterAttribute parameterAttribute)
        {
            EventDtoProperty = eventDtoProperty;
            ParameterAttribute = parameterAttribute;
        }

        public PropertyInfo EventDtoProperty { get; }
        public ParameterAttribute ParameterAttribute { get; }

        public object[] GetValues()
        {
            return _values == null || _values.Count == 0 ? null : _values.ToArray();
        }

        public void AddValue(object val)
        {
            if (_values == null)
            {
                _values = new List<object>();
            }
            _values.Add(val);
        }
    }
}