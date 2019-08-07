using Microsoft.Azure.Search.Models;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Nethereum.BlockchainStore.Search.Azure
{
    public static class AzureEventSearchExtensions
    {
        public const string SuggesterName = "sg";

        public static Dictionary<string, object> ToAzureDocument<TFunctionMessage>(
            this TransactionForFunctionVO<TFunctionMessage> transactionAndFunction,
            FunctionIndexDefinition<TFunctionMessage> indexDefinition)
            where TFunctionMessage : FunctionMessage, new()
        {
            var dictionary = new Dictionary<string, object>();
            foreach (var field in indexDefinition.Fields)
            {
                var azureField = field.ToAzureField();

                var val = field.GetValue(transactionAndFunction)?.ToAzureFieldValue();
                if (val != null)
                {
                    dictionary.Add(azureField.Name, val);
                }
            }

            return dictionary;
        }

        public static Dictionary<string, object> ToAzureDocument<TEvent>(this EventLog<TEvent> log, EventIndexDefinition<TEvent> indexDefinition) where TEvent : class
        {
            var dictionary = new Dictionary<string, object>();
            foreach (var field in indexDefinition.Fields)
            {
                var azureField = field.ToAzureField();

                var val = field.GetValue(log)?.ToAzureFieldValue();
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