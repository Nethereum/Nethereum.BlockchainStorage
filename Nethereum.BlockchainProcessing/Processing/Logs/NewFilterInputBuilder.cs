using Nethereum.ABI.Model;
using Nethereum.Contracts;
using Nethereum.Contracts.Extensions;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

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
    public class NewFilterInputBuilder<TEventDTo> where TEventDTo : class
    {
        private readonly EventABI _eventAbi;
        private readonly TopicFilterContainer<TEventDTo> _topics;

        public NewFilterInputBuilder()
        {
            _eventAbi = ABITypedRegistry.GetEvent<TEventDTo>();
            _topics = new TopicFilterContainer<TEventDTo>();
        }

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

            _topics
                .GetTopic(propertyInfo)
                .AddValue(desiredValue);

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

            if (_topics.Empty)
            {
                return _eventAbi.CreateFilterInput(contractAddresses, from, to);
            }

            return _eventAbi.CreateFilterInput(
                contractAddresses, 
                _topics.Topic1.GetValues(), 
                _topics.Topic2.GetValues(), 
                _topics.Topic3.GetValues(), 
                from, 
                to);
        }
    }
}