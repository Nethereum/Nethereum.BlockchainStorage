using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Nethereum.ABI;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.FunctionEncoding.AttributeEncoding;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.ABI.Model;
using Nethereum.ABI.Util;
using Nethereum.Contracts;
using Nethereum.Contracts.Extensions;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    /// <summary>
    /// Builds a filter based on indexed parameters on an event DTO query template.
    /// The DTO should have properties decorated with ParameterAttribute
    /// Only ParameterAttributes flagged as indexed are included
    /// Use AddCondition to set a value on a indexed property on the query template
    /// Values set on the query template are put in to the filter when Build is called
    /// </summary>
    /// <typeparam name="TEventDTo"></typeparam>
    public class NewFilterInputBuilder<TEventDTo> where TEventDTo : new()
    {
        private readonly AttributesToABIExtractor _attributesToAbiExtractor;
        private readonly ParametersEncoder _parametersEncoder;
        private readonly EventABI _eventAbi;
        private readonly IEnumerable<PropertyInfo> _eventDtoProperties;

        public NewFilterInputBuilder()
        {
            Template = new TEventDTo();
            _eventAbi = ABITypedRegistry.GetEvent<TEventDTo>();
            _eventDtoProperties = PropertiesExtractor.GetPropertiesWithParameterAttribute(typeof(TEventDTo));
            _attributesToAbiExtractor = new AttributesToABIExtractor();
            _parametersEncoder = new ParametersEncoder();
        }

        private TEventDTo Template { get; }

        /// <summary>
        /// Prevents zeros (which are often defaults on BigInteger structs or numeric value types) from appearing in the filter
        /// If these are not ignored the filter can contain an equals 0 condition
        /// This condition may exclude desired logs from being returned
        /// Default is true
        /// </summary>
        public bool IgnoreZeros { get; set; } = true;

        /// <summary>
        /// Set the desired filter value on one of the indexed properties of the object
        /// </summary>
        /// <param name="setValueOnIndexedField">An action to set the value on the query template.
        /// Set a property representing an indexed parameter of the event
        /// e.g. (queryTemplate) => queryTemplate.From = "xyz"</param>
        public NewFilterInputBuilder<TEventDTo> AddCondition(Action<TEventDTo> setValueOnIndexedField)
        {
            setValueOnIndexedField(Template);
            return this;
        }

        public NewFilterInput Build(string[] contractAddresses = null, BlockParameter from = null, BlockParameter to = null)
        {
            var indexedParameterValues = GetIndexedParameterValues(Template);

            if (IgnoreZeros)
            {
                NullifyZeros(indexedParameterValues);
            }

            if (indexedParameterValues.Length == 0 || indexedParameterValues.All(p => p.Value == null))
            {
                return _eventAbi.CreateFilterInput(contractAddresses, from, to);
            }

            var topic1 = indexedParameterValues[0].Value;
            var topic2 = indexedParameterValues.Length > 0 ? indexedParameterValues[1].Value : null;
            var topic3 = indexedParameterValues.Length > 1 ? indexedParameterValues[2].Value : null;

            return _eventAbi.CreateFilterInput(contractAddresses, topic1, topic2, topic3, from, to);
        }

        private static void NullifyZeros(ParameterAttributeValue[] indexedParameterValues)
        {
            foreach (var indexedParameter in indexedParameterValues.Where(p => p.Value != null))
            {
                if (indexedParameter.Value is BigInteger bigInt)
                {
                    if (bigInt.IsZero)
                    {
                        indexedParameter.Value = null;
                        break;
                    }
                }

                if (indexedParameter.Value.GetType().IsNumber() && (long)indexedParameter.Value == 0)
                {
                    indexedParameter.Value = null;
                }
            }
        }

        private ParameterAttributeValue[] GetIndexedParameterValues(object instanceValue)
        {
            var parameterObjects = new List<ParameterAttributeValue>();

            foreach (var property in _eventDtoProperties)
            {
                var parameterAttribute = property.GetCustomAttribute<ParameterAttribute>(true);

                if(!parameterAttribute.Parameter.Indexed) continue;
                
                var propertyValue = property.GetValue(instanceValue);

                if (parameterAttribute.Parameter.ABIType is TupleType tupleType)
                {
                    _attributesToAbiExtractor.InitTupleComponentsFromTypeAttributes(property.PropertyType, tupleType);
                    propertyValue = _parametersEncoder.GetTupleComponentValuesFromTypeAttributes(property.PropertyType, propertyValue);
                }

                parameterObjects.Add(new ParameterAttributeValue
                {
                    ParameterAttribute = parameterAttribute,
                    Value = propertyValue
                });
            }

            return 
                parameterObjects.OrderBy(x => x.ParameterAttribute.Order)
                    .ToArray();
        }
    }
}