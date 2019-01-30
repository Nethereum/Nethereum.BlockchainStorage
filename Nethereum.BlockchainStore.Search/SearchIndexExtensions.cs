using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nethereum.BlockchainStore.Search
{
    public static class SearchIndexExtensions
    {
        public static SearchField Field(this SearchIndexDefinition searchIndex, string name)
        {
            return searchIndex.Fields.FirstOrDefault(f => f.Name == name);
        }

        public static SearchField Field(this SearchIndexDefinition searchIndex, PresetSearchFieldName name)
        {
            return searchIndex.Field(name.ToString());
        }

        public static SearchField KeyField(this SearchIndexDefinition searchIndex)
        {
            return searchIndex.Fields.FirstOrDefault(f => f.IsKey);
        }

        public static bool IsGenericList(this Type type)
        {
            return (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>)));
        }

        public static bool IsArrayOrList(this Type type)
        {
            return type.IsArray || type.IsGenericList();
        }

        public static bool IsArrayOrList(this PropertyInfo propertyInfo)
        {
            return propertyInfo.PropertyType.IsArrayOrList();
        }

        public static Type GetArrayType(this PropertyInfo property)
        {
            return property.PropertyType.GetArrayType();
        }

        public static Type GetArrayType(this Type arrayOrListType)
        {
            if (arrayOrListType.IsArray) return arrayOrListType.GetElementType();
            if (arrayOrListType.IsGenericList())
                return arrayOrListType.GetGenericArguments()[0];
            return null;
        }

        public static bool IsArrayOrList(this object val)
        {
            return val?.GetType().IsArrayOrList() ?? false;
        }

        public static bool IsArrayOrList(this object arrayOrList, out IEnumerable val)
        {
            val = null;
            if (!arrayOrList.IsArrayOrList()) return false;
            val = (IEnumerable) arrayOrList;
            return true;
        }

        public static Array GetPropertyValues(this IEnumerable arrayOrList, PropertyInfo propertyInfo)
        {
            var itemType = arrayOrList.GetType().GetElementType();

            if (itemType?.IsValueType ?? false)
            {
                return arrayOrList as Array;
            }

            if (arrayOrList is Array array)
            {
                //we want an array of property values from each member of the array

                var values = Array.CreateInstance(propertyInfo.PropertyType, array.Length);

                for (var i = 0; i < array.Length; i++)
                {
                    values.SetValue(propertyInfo.GetValue(array.GetValue(i)), i);
                }

                return values;
            }

            if (arrayOrList is IList list)
            {
                var values = Array.CreateInstance(propertyInfo.PropertyType, list.Count);

                for (var i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    values.SetValue(propertyInfo.GetValue(item), i);
                }

                return values;
            }

            return null;
        }

        public static Array GetElementsAsArray(this IEnumerable arrayOrList)
        {
            if (arrayOrList is Array array)
            {
                return array;
            }

            if (arrayOrList is IList list)
            {
                var type = arrayOrList.GetType().GetArrayType();

                var objectArray = Array.CreateInstance(type, list.Count);

                for (var i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    objectArray.SetValue(item, i);
                }

                return objectArray;
            }

            return null;
        }

    }
}