using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Linq;
using System.Reflection;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public static class EventTopicUtils
    {
        public static ParameterAttributeIndexedTopics[] GetIndexedTopics<T>()
        {
             return PropertiesExtractor
                .GetPropertiesWithParameterAttribute(typeof(T))
                .Select(p => new ParameterAttributeIndexedTopics
                {
                    ParameterAttribute = p.GetCustomAttribute<ParameterAttribute>(),
                    PropertyInfo = p
                })
                .Where(p => p.ParameterAttribute?.Parameter.Indexed ?? false)
                .OrderBy(p => p.ParameterAttribute.Order)
                .ToArray();
        }
    }

}