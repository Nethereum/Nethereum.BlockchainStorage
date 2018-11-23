using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
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
    /// Use SetTopic to set a value on a indexed property on the query template
    /// Values set on the query template are put in to the filter when Build is called
    /// </summary>
    /// <typeparam name="TEventDTo"></typeparam>
    public class NewFilterInputBuilder<TEventDTo> where TEventDTo : new()
    {
        private readonly AttributesToABIExtractor _attributesToAbiExtractor;
        private readonly ParametersEncoder _parametersEncoder;
        private readonly EventABI _eventAbi;
        private readonly ParameterAttribute[] _indexedParameters;

        private readonly Dictionary<string, List<object>> _topicValuesDictionary;

        public NewFilterInputBuilder()
        {
            Template = new TEventDTo();
            _eventAbi = ABITypedRegistry.GetEvent<TEventDTo>();

            _indexedParameters = PropertiesExtractor
                .GetPropertiesWithParameterAttribute(typeof(TEventDTo))
                .Select(p => p.GetCustomAttribute<ParameterAttribute>())
                .Where(p => p?.Parameter.Indexed ?? false)
                .OrderBy(p => p.Order)
                .ToArray();
                
            _attributesToAbiExtractor = new AttributesToABIExtractor();
            _parametersEncoder = new ParametersEncoder();
            _topicValuesDictionary = new Dictionary<string, List<object>>(_indexedParameters.Count());
        }

        private TEventDTo Template { get; }

        public NewFilterInputBuilder<TEventDTo> AddTopic<TPropertyType>(
            Expression<Func<TEventDTo, TPropertyType>> propertySelector, IEnumerable<TPropertyType> desiredValues)
        {
            foreach (var desiredValue in desiredValues)
            {
                AddTopic(propertySelector, desiredValue);
            }

            return this;
        }

        public NewFilterInputBuilder<TEventDTo> AddTopic<TPropertyType>(
            Expression<Func<TEventDTo, TPropertyType>> propertySelector, TPropertyType desiredValue)
        {
            var member = propertySelector.Body as MemberExpression; 
            var propertyInfo = member?.Member as PropertyInfo;

            var parameterAttribute = propertyInfo?.GetCustomAttribute<ParameterAttribute>(true);

            if(parameterAttribute == null || !parameterAttribute.Parameter.Indexed)
                throw new ArgumentException("Property must have attribute ParameterAttribute flagged as indexed");

            var key = CreateKey(parameterAttribute);

            if (!_topicValuesDictionary.ContainsKey(key))
            {
                _topicValuesDictionary.Add(key, new List<object>());
            }

            object valueToStore = desiredValue;

            if (parameterAttribute.Parameter.ABIType is TupleType tupleType)
            {
                _attributesToAbiExtractor.InitTupleComponentsFromTypeAttributes(propertyInfo.PropertyType, tupleType);
                valueToStore = _parametersEncoder.GetTupleComponentValuesFromTypeAttributes(propertyInfo.PropertyType, desiredValue);
            }

            _topicValuesDictionary[key].Add(valueToStore);

            return this;
        }

        public NewFilterInput Build(string contractAddress, BlockRange? blockRange = null)
        {
            return Build(new[] {contractAddress}, blockRange);
        }

        public NewFilterInput Build(string[] contractAddresses = null, BlockRange? blockRange = null)
        {
            BlockParameter from = blockRange == null ? null : new BlockParameter(blockRange.Value.From);
            BlockParameter to = blockRange == null ? null : new BlockParameter(blockRange.Value.To);

            if (!_indexedParameters.Any() || _topicValuesDictionary.Count == 0)
            {
                return _eventAbi.CreateFilterInput(contractAddresses, from, to);
            }

            object[] topic1 = null;
            object[] topic2 = null;
            object[] topic3 = null;

            string key = null;

            if (_indexedParameters.Length > 0)
            {
                var topic1Parameter = _indexedParameters[0];
                key = CreateKey(topic1Parameter);

                topic1 = _topicValuesDictionary.ContainsKey(key)
                    ? _topicValuesDictionary[key].ToArray()
                    : Array.Empty<object>();
            }

            if (_indexedParameters.Length > 1)
            {
                var topic2Parameter = _indexedParameters[1];
                key = CreateKey(topic2Parameter);

                topic2 = _topicValuesDictionary.ContainsKey(key)
                    ? _topicValuesDictionary[key].ToArray()
                    : Array.Empty<object>();
            }

            if (_indexedParameters.Length > 2)
            {
                var topic3Parameter = _indexedParameters[2];
                key = CreateKey(topic3Parameter);

                topic3 = _topicValuesDictionary.ContainsKey(key)
                    ? _topicValuesDictionary[key].ToArray()
                    : Array.Empty<object>();
            }

            return _eventAbi.CreateFilterInput(
                contractAddresses, 
                topic1 ?? Array.Empty<object>(), 
                topic2 ?? Array.Empty<object>(), 
                topic3 ?? Array.Empty<object>(), 
                from, 
                to);
        }

        private string CreateKey(ParameterAttribute parameterAttribute)
        {
            return string.IsNullOrWhiteSpace(parameterAttribute.Name)
                ? parameterAttribute.Order.ToString()
                : parameterAttribute.Name;
        }
    }
}