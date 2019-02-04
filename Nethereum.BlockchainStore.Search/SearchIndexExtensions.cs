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

        public static bool IsListOfT(this Type type)
        {
            var underlyingListType = type.GetUnderlyingGenericListType();
            return underlyingListType != null;
        }

        public static bool IsArrayOrListOfT(this Type type)
        {
            return type.IsArray || type.IsListOfT();
        }

        public static bool IsPresetSearchField(this SearchField field)
        {
            return Enum.TryParse(field.Name, out PresetSearchFieldName val);
        }

        public static bool IsArrayOrListOfT(this PropertyInfo propertyInfo)
        {
            return propertyInfo.PropertyType.IsArrayOrListOfT();
        }

        public static Type GetItemTypeFromArrayOrListOfT(this PropertyInfo property)
        {
            return property.PropertyType.GetItemTypeFromArrayOrListOfT();
        }

        public static Type GetItemTypeFromListOfT(this Type listType)
        {
            var underlyingListType = listType.GetUnderlyingGenericListType();
            return underlyingListType == null ? 
                null : 
                underlyingListType.GetGenericArguments().FirstOrDefault();
        }

        public static Type GetItemTypeFromArrayOrListOfT(this Type arrayOrListType)
        {
            if (arrayOrListType.IsArray) return arrayOrListType.GetElementType();
            if (arrayOrListType.IsListOfT())
                return arrayOrListType.GetItemTypeFromListOfT();
            return null;
        }

        public static bool IsArrayOrListOfT(this object val)
        {
            return val?.GetType().IsArrayOrListOfT() ?? false;
        }

        public static bool IsArrayOrListOfT(this object arrayOrList, out IEnumerable val)
        {
            val = null;
            if (!arrayOrList.IsArrayOrListOfT()) return false;
            val = (IEnumerable) arrayOrList;
            return true;
        }

        public static Array GetAllElementPropertyValues(this IEnumerable arrayOrList, PropertyInfo propertyInfo)
        {
            if(propertyInfo == null) throw new ArgumentNullException(nameof(propertyInfo));

            var itemType = arrayOrList.GetType().GetItemTypeFromArrayOrListOfT();

            if (itemType == null) return null;

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

        public static Array GetItems(this IEnumerable arrayOrList)
        {
            if (arrayOrList == null) return null;

            if (arrayOrList is Array array)
            {
                return array;
            }

            if (arrayOrList is IList list)
            {
                var type = arrayOrList.GetType().GetItemTypeFromListOfT();

                if (type == null) return null;

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

        private static Type GetUnderlyingGenericListType(this Type listType)
        {
            do
            {
                if (listType.IsListOfT_WithoutInheritanceChecks())
                {
                    return listType;
                }

                listType = listType.BaseType;
            } while (listType != null && listType != typeof(object));

            return null;
        }

        private static bool IsListOfT_WithoutInheritanceChecks(this Type type)
        {
            return type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>));
        }

    }
}