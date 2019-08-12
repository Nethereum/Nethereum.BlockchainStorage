using Microsoft.Azure.Search.Models;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public static class AzureSearchExtensions
    {
        public const string SuggesterName = "sg";

        public static GenericSearchDocument ToAzureDocument(
            this FilterLog log,
            SearchField[] searchFields)
        {
            return CreateFieldWithValueDictionary(log, searchFields, (field) => field.GetFilterLogValue(log));
        }

        public static GenericSearchDocument ToAzureDocument<TEvent>(
            this EventLog<TEvent> log, 
            EventIndexDefinition<TEvent> indexDefinition) where TEvent : class
        {
            return CreateFieldWithValueDictionary(log, indexDefinition.Fields, (field) => field.GetEventLogValue(log));
        }

        public static GenericSearchDocument ToAzureDocument<TFunctionMessage>(
            this TransactionForFunctionVO<TFunctionMessage> transactionAndFunction,
            FunctionIndexDefinition<TFunctionMessage> indexDefinition)
            where TFunctionMessage : FunctionMessage, new()
        {
            return CreateFieldWithValueDictionary(transactionAndFunction, indexDefinition.Fields, (field) => field.GetTransactionForFunctionValue(transactionAndFunction));
        }

        public static GenericSearchDocument ToAzureDocument(
            this TransactionReceiptVO transactionReceiptVO,
            TransactionReceiptVOIndexDefinition indexDefinition)
        {
            return CreateFieldWithValueDictionary(transactionReceiptVO, indexDefinition.Fields, (field) => field.GetTransactionReceiptValue(transactionReceiptVO));
        }

        private static GenericSearchDocument CreateFieldWithValueDictionary<T>(T source, SearchField[] searchFields, Func<SearchField, object> getValue)
        {
            var dictionary = new GenericSearchDocument();
            foreach (var field in searchFields)
            {
                var azureField = field.ToAzureField();

                var val = getValue.Invoke(field)?.ToAzureFieldValue();
                if (val != null)
                {
                    dictionary.Add(azureField.Name, val);
                }
            }

            return dictionary;
        }

        public static Index ToAzureIndex(this IndexDefinition searchIndex)
        {
            var index = new Index
            {
                Name = searchIndex.IndexName.ToAzureIndexName(), 
                Fields = searchIndex.Fields.ToAzureFields(), 
                Suggesters = searchIndex.Fields.ToAzureSuggesters()
            };

            return index;
        }

        public static string ToAzureIndexName(this string indexName)
        {
            return indexName.ToLower().Replace(".", "_");
        }

        public static Suggester[] ToAzureSuggesters(this IEnumerable<SearchField> fields)
        {
            var suggesterFields = fields
                .Where(f => f.IsSuggester && f.IsSearchable)
                .OrderBy(f => f.SuggesterOrder)
                .ToArray();

            if (!suggesterFields.Any()) return Array.Empty<Suggester>();
            return new[] {new Suggester(SuggesterName, suggesterFields.Select(f => f.Name.ToLower()).ToArray())};
        }

        public static Field[] ToAzureFields(this IEnumerable<SearchField> fields)
        {
            return fields.Select(ToAzureField).ToArray();
        }

        public static Field ToAzureField(this SearchField f)
        {
            return new Field(f.Name.ToAzureFieldName(), f.IsCollection ? DataType.Collection(f.DataType.ToAzureDataType()) : f.DataType.ToAzureDataType())
            {
                IsKey = f.IsKey,
                IsFilterable = f.IsFilterable,
                IsSortable = f.IsSortable,
                IsSearchable = f.IsSearchable,
                IsFacetable = f.IsFacetable,
            };
        }

        public static string ToAzureFieldName(this string fieldName)
        {
            return fieldName.ToLower().Replace(".", "_");
        }

        public static object ToAzureFieldValue(this object val)
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

        public static DataType ToAzureDataType(this Type type)
        {
            if(type == typeof(string)) return DataType.String;
            if(type == typeof(bool)) return DataType.Boolean;
            if(type == typeof(HexBigInteger)) return DataType.String;
            if(type == typeof(BigInteger)) return DataType.String;
            if (type.IsArray) return DataType.Collection(type.GetElementType().ToAzureDataType());
            if (type.IsListOfT()) return DataType.Collection(type.GetGenericArguments()[0].ToAzureDataType());

            return DataType.String;
        }

        public static IList<string> FacetableFieldNames(this Index index)
        {
            return index.Fields.Where(f => f.IsFacetable).Select(f => f.Name).ToList();
        }
    }
}