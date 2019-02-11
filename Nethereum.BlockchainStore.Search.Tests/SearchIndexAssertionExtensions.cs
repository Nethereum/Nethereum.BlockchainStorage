using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System;
using Xunit;
using Xunit.Sdk;

namespace Nethereum.BlockchainStore.Search.Tests
{
    public abstract class Context
    {
        public IndexDefinition Index;

        protected Context(IndexDefinition index)
        {
            Index = index;
        }

        public abstract object GetValue(SearchField field);
    }

    public class TransactionContext : Context
    {
        public object Dto;
        public Transaction Tx;

        public TransactionContext(IndexDefinition index, object dto, Transaction tx)
            :base(index)
        {
            Tx = tx;
            Dto = dto;
            Index = index;
        }

        public override object GetValue(SearchField field)
        {
            return field.GetValue(Dto, Tx);
        }
    }

    public class EventLogContext<TEvent> : Context
    {
        public EventLog<TEvent> EventLog;

        public EventLogContext(IndexDefinition index, EventLog<TEvent> eventLog)
            :base(index)
        {
            EventLog = eventLog;
            Index = index;
        }

        public override object GetValue(SearchField field)
        {
            return field.GetValue(EventLog);
        }
    }


    public static class SearchIndexAssertionExtensions
    {

        public static TransactionContext Assertions(this IndexDefinition index, object dto, Transaction tx)
        {
            return new TransactionContext(index, dto, tx);
        }

        public static EventLogContext<TEvent> Assertions<TEvent>(this IndexDefinition index, EventLog<TEvent> eventLog)
        {
            return new EventLogContext<TEvent>(index, eventLog);
        }

        public static (Context Context, SearchField field) HasField(
            this Context context, PresetSearchFieldName fieldName)
        {
            var field = context.Index.Field(fieldName);
            try
            {
                Assert.NotNull(field);
                return (context, field);
            }            
            catch (XunitException ex)
            {
                throw new Exception($"Field null expectation failure: '{fieldName}'. {ex.Message}",
                    ex);
            }
        }

        public static (Context Context, SearchField field) HasField(
            this Context context, string fieldName)
        {
            var field = context.Index.Field(fieldName);
            try
            {
                Assert.NotNull(field);
                return (context, field);
            }            
            catch (XunitException ex)
            {
                throw new Exception($"Field null expectation failure: '{fieldName}'. {ex.Message}",
                    ex);
            }
        }

        public static Context HasField(
            this Context context, string fieldName, Action<(Context, SearchField)> assertions)
        {
            var field = context.Index.Field(fieldName);
            try
            {
                Assert.NotNull(field);
                assertions((context, field));
                return context;
            }            
            catch (XunitException ex)
            {
                throw new Exception($"Field null expectation failure: '{fieldName}'. {ex.Message}",
                    ex);
            }
        }

        public static Context HasField(
            this Context context, string fieldName, params Action<(Context, SearchField)>[] assertions)
        {
            var field = context.Index.Field(fieldName);
            try
            {
                Assert.NotNull(field);
                foreach (var assertion in assertions)
                {
                    assertion((context, field));
                }

                return context;
            }            
            catch (XunitException ex)
            {
                throw new Exception($"Field null expectation failure: '{fieldName}'. {ex.Message}",
                    ex);
            }
        }

        public static Context HasField(
            this Context context, PresetSearchFieldName fieldName, params Action<(Context, SearchField)>[] assertions)
        {
            var field = context.Index.Field(fieldName);
            try
            {
                Assert.NotNull(field);
                foreach (var assertion in assertions)
                {
                    assertion((context, field));
                }

                return context;
            }            
            catch (XunitException ex)
            {
                throw new Exception($"Field null expectation failure: '{fieldName}'. {ex.Message}",
                    ex);
            }
        }

        public static (Context Context, SearchField field) ReturnsValue(this (Context, SearchField) ctx, object expectedValue)
        {
            try
            {
                Assert.Equal(expectedValue, ctx.Item1.GetValue(ctx.Item2));
                return ctx;
            }
            catch (XunitException ex)
            {
                throw new Exception($"Field 'GetValue' expectation failure: '{ctx.Item2.Name}'. {ex.Message}",
                    ex);
            }
        }

        public static (Context Context, SearchField field) ReturnsValue<T>(this (Context, SearchField) ctx, Func<T, (object, object)> expectedAndActualValueRetriever)
        {
            try
            {
                var val = (T)ctx.Item1.GetValue(ctx.Item2); 
                Assert.NotNull(val);

                var pair = expectedAndActualValueRetriever(val);
                Assert.Equal(pair.Item1, pair.Item2);
                return ctx;
            }
            catch (XunitException ex)
            {
                throw new Exception($"Field 'GetValue' expectation failure: '{ctx.Item2.Name}'. {ex.Message}",
                    ex);
            }
        }

        public static (Context Context, SearchField field) IsString(this (Context, SearchField) ctx)
        {
            return ctx.DataType<string>();
        }

        public static (Context Context, SearchField field) IsHexBigInteger(this (Context, SearchField) ctx)
        {
            return ctx.DataType<HexBigInteger>();
        }


        public static (Context Context, SearchField field) DataType<TExpectedType>(this (Context, SearchField) ctx)
        {
            return ctx.DataType(typeof(TExpectedType));
        }

        public static (Context Context, SearchField field) DataType(this (Context, SearchField) ctx, Type expectedType)
        {
            try
            {
                Assert.Equal(expectedType, ctx.Item2.DataType);
                return ctx;
            }
            catch (XunitException ex)
            {
                throw new Exception($"Field 'GetValue' DataType expectation failure '{ctx.Item2.Name}'. {ex.Message}",
                    ex);
            }
        }

        public static (Context Context, SearchField field) IsCollection(
            this (Context, SearchField) ctx)
        {
            try
            {
                Assert.True(ctx.Item2.IsCollection);
                return ctx;
            }
            catch (XunitException ex)
            {
                throw new Exception($"Field attribute expectation failure: '{ctx.Item2.Name}':'IsCollection'. {ex.Message}",
                    ex);
            }
        }

        public static (Context Context, SearchField field) HasFlags(
            this (Context, SearchField) ctx,
            bool isKey = false,
            bool isSortable = false,
            bool isSearchable = false,
            bool ignore = false,
            bool isFilterable = false,
            bool isFacetable = false,
            bool isCollection = false)
        {
            string property = "IsKey";
            try
            {
                var field = ctx.Item2;
                Assert.Equal(isKey, field.IsKey);
                property = "IsSortable";
                Assert.Equal(isSortable, field.IsSortable);
                property = "IsSearchable";
                Assert.Equal(isSearchable, field.IsSearchable);
                property = "Ignore";
                Assert.Equal(ignore, field.Ignore);
                property = "IsFilterable";
                Assert.Equal(isFilterable, field.IsFilterable);
                property = "IsFacetable";
                Assert.Equal(isFacetable, field.IsFacetable);
                property = "IsCollection";
                Assert.Equal(isCollection, field.IsCollection);

                return ctx;
            }
            catch (XunitException ex)
            {
                throw new Exception($"Field attribute expectation failure: '{ctx.Item2.Name}':'{property}'. {ex.Message}",
                    ex);
            }
        }
    }
}
