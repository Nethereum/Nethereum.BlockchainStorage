using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Nethereum.ABI.FunctionEncoding.AttributeEncoding;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.ABI.Model;
using Nethereum.ABI.Util;
using Nethereum.Contracts;
using Nethereum.Contracts.Extensions;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public class NewFilterInputBuilder<TEventDTo> where TEventDTo : new()
    {
        private readonly EventABI _eventAbi;
        private readonly IEnumerable<PropertyInfo> _eventDtoProperties;

        public NewFilterInputBuilder()
        {
            Template = new TEventDTo();
            _eventAbi = ABITypedRegistry.GetEvent<TEventDTo>();
            _eventDtoProperties = PropertiesExtractor.GetPropertiesWithParameterAttribute(typeof(TEventDTo));
        }

        private TEventDTo Template { get; }

        /// <summary>
        /// Prevents zeros (which are often defaults for BigIntegers and number types) from appearing in the filter
        /// Default is true
        /// </summary>
        public bool IgnoreZeros { get; set; } = true;

        public NewFilterInputBuilder<TEventDTo> AddCondition(Action<TEventDTo> setTemplateAction)
        {
            setTemplateAction(Template);
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

            var indexedVal1 = indexedParameterValues[0].Value;
            var indexedVal2 = indexedParameterValues.Length > 0 ? indexedParameterValues[1].Value : null;
            var indexedVal3 = indexedParameterValues.Length > 1 ? indexedParameterValues[2].Value :  null;

            return _eventAbi.CreateFilterInput(contractAddresses, indexedVal1, indexedVal2, indexedVal3, from, to);
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

                //if (parameterAttribute.ParameterABIType is TupleType tupleType)
                //{
                //    attributesToABIExtractor.InitTupleComponentsFromTypeAttributes(property.PropertyType, tupleType);
                //    propertyValue = GetTupleComponentValuesFromTypeAttributes(property.PropertyType, propertyValue);
                //}

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