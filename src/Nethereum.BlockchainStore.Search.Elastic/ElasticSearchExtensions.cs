﻿using System;
using Nethereum.Contracts;
using System.Numerics;
using System.Text;
using Nest;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Search.ElasticSearch
{

    public static class ElasticSearchExtensions
    {
        public static GenericSearchDocument ToGenericElasticSearchDoc(
            this TransactionReceiptVO transactionReceiptVO,
            TransactionReceiptVOIndexDefinition indexDefinition)
        {
            var dictionary = new GenericSearchDocument();
            foreach (var field in indexDefinition.Fields)
            {
                var val = field.GetTransactionReceiptValue(transactionReceiptVO)?.ToElasticSearchFieldValue();
                if (val != null)
                {
                    dictionary.Add(field.Name.ToElasticName(), val);
                }
            }

            var id = transactionReceiptVO.TransactionHash;
            dictionary.SetId(id.ToString());

            return dictionary;
        }

        public static GenericSearchDocument ToGenericElasticSearchDoc(
            this FilterLog log,
            SearchField[] searchFields)
        {
            var dictionary = new GenericSearchDocument();
            foreach (var field in searchFields)
            {
                var val = field.GetFilterLogValue(log)?.ToElasticSearchFieldValue();
                if (val != null)
                {
                    dictionary.Add(field.Name.ToElasticName(), val);
                }
            }

            var id = log.Key();
            dictionary.SetId(id.ToString());

            return dictionary;
        }

        public static GenericSearchDocument ToGenericElasticSearchDoc<TEvent>(
            this EventLog<TEvent> log, 
            EventIndexDefinition<TEvent> indexDefinition) where TEvent : class
        {
            var dictionary = new GenericSearchDocument();
            foreach (var field in indexDefinition.Fields)
            {
                var val = field.GetEventLogValue(log)?.ToElasticSearchFieldValue();
                if (val != null)
                {
                    dictionary.Add(field.Name.ToElasticName(), val);
                }
            }

            var id = indexDefinition.KeyField().GetEventLogValue(log);
            dictionary.SetId(id.ToString());

            return dictionary;
        }

        public static GenericSearchDocument ToGenericElasticSearchDoc<TFunctionMessage>(
            this TransactionForFunctionVO<TFunctionMessage> functionCall, 
            FunctionIndexDefinition<TFunctionMessage> indexDefinition) where TFunctionMessage : FunctionMessage, new()
        {
            var dictionary = new GenericSearchDocument();
            foreach (var field in indexDefinition.Fields)
            {
                var val = field.GetTransactionForFunctionValue(functionCall)?.ToElasticSearchFieldValue();
                if (val != null)
                {
                    dictionary.Add(field.Name.ToElasticName(), val);
                }
            }

            var id = indexDefinition.KeyField().GetTransactionForFunctionValue(functionCall);
            dictionary.SetId(id.ToString());

            return dictionary;
        }

        public static string ElasticIndexName(
            this IndexDefinition indexDefinition)
        {
            return indexDefinition.IndexName.ToElasticName();
        }


        public static string ToElasticName(this string name)
        {
            return name.ToLowerInvariant();
        }

        public static object ToElasticSearchFieldValue(this object val)
        {
            if (val == null) return null;

            if(val is string) return val;
            if(val is bool) return val;
            if(val is HexBigInteger hexBigInteger) return hexBigInteger.Value.ToString();
            if(val is BigInteger bigInteger) return bigInteger.ToString();
            if (val is byte[] byteArray) return Encoding.UTF8.GetString(byteArray);

            if (val.GetType().IsArrayOrListOfT())
            {
                return val;
            }

            return val.ToString();
        }

        public static IProperty ToElasticProperty(this Type type)
        {
            if (type == typeof(string)) return new TextProperty();
            if(type == typeof(bool)) return new BooleanProperty();
            if(type == typeof(HexBigInteger)) return new TextProperty();
            if(type == typeof(BigInteger)) return new TextProperty();
            if (type.IsArray) return new TextProperty();
            if (type.IsListOfT()) return new TextProperty();

            return new TextProperty();
        }

        public static Mappings CreateElasticMappings(this IndexDefinition eventIndexDefinition, bool allowDynamicMapping = false)
        {
            var properties = new Properties();

            foreach (var field in eventIndexDefinition.Fields)
            {
                properties.Add(field.Name, field.DataType.ToElasticProperty());
            }

            var mappings = new Mappings
            {
                {
                    "_doc",
                    new TypeMapping()
                    {
                        Dynamic = new Union<bool, DynamicMapping>(allowDynamicMapping),
                        Properties = properties
                    }
                }
            };

            return mappings;
        }
    }
}
