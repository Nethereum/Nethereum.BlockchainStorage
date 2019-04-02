using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Nethereum.BlockchainStore.Search.Tests
{
    public class SearchIndexExtensionTests
    {
        public class CustomStringList : List<string>{}

        public class CustomStringListA : CustomStringList{}

        public class Dto
        {
            public string Name { get;set; }
        }

        [Fact]
        public void IsGenericList()
        {
            Assert.False(typeof(object[]).IsListOfT(), "array should not be treated as a generic list");
            Assert.False(typeof(string).IsListOfT(), "strings should not be treated as a generic list");

            Assert.True(typeof(List<string>).IsListOfT(), "List<string> is a generic list");
            Assert.True(typeof(List<Dto>).IsListOfT(), "List<Dto> is a generic list");
            Assert.True(typeof(CustomStringList).IsListOfT(), "CustomList is a generic list");
            Assert.True(typeof(CustomStringListA).IsListOfT(), "CustomListA inherits a generic list");
        }


        [Fact]
        public void IsArrayOrList()
        {
            Assert.True(typeof(object[]).IsArrayOrListOfT(), "object[] is an array");
            Assert.True(typeof(List<string>).IsArrayOrListOfT(), "List<string> is a generic list");
            Assert.True(typeof(List<Dto>).IsArrayOrListOfT(), "List<Dto> is a generic list");
            Assert.True(typeof(CustomStringList).IsArrayOrListOfT(), "CustomList is a generic list");
            Assert.True(typeof(CustomStringListA).IsArrayOrListOfT(), "CustomListA inherits a generic list");

            Assert.False(typeof(ArrayList).IsArrayOrListOfT());
            Assert.False(typeof(string).IsArrayOrListOfT());
            Assert.False(typeof(Dictionary<string, object>).IsArrayOrListOfT());
        }

        [Fact]
        public void IsArrayOrListOfT_WithEnumerableOutParameter()
        {
            var list = new List<string>();
            if (list.IsArrayOrListOfT(out IEnumerable enumerable))
            {
                Assert.NotNull(enumerable);
                Assert.Same(list, enumerable);
            }

            var array = new string[0];
            if (array.IsArrayOrListOfT(out IEnumerable arrayEnumerable))
            {
                Assert.NotNull(arrayEnumerable);
                Assert.Same(array, arrayEnumerable);
            }

            var dtoArray = new Dto[0];
            if (dtoArray.IsArrayOrListOfT(out IEnumerable dtoEnumerable))
            {
                Assert.NotNull(dtoEnumerable);
                Assert.Same(dtoArray, dtoEnumerable);
            }

            Assert.False("test".IsArrayOrListOfT(out IEnumerable stringEnumerable));
        }

        [Fact]
        public void GetItemTypeFromListOfT()
        {
            Assert.Equal(typeof(int), typeof(List<int>).GetItemTypeFromListOfT());
            Assert.Equal(typeof(string), typeof(List<string>).GetItemTypeFromListOfT());
            Assert.Equal(typeof(object), typeof(List<object>).GetItemTypeFromListOfT());
            Assert.Equal(typeof(string), typeof(CustomStringList).GetItemTypeFromListOfT());
            Assert.Equal(typeof(Dto), typeof(List<Dto>).GetItemTypeFromListOfT());

            Assert.Null(typeof(string[]).GetItemTypeFromListOfT());
            Assert.Null(typeof(ArrayList).GetItemTypeFromListOfT());
        }

        [Fact]
        public void GetItemTypeFromArrayOrList()
        {
            Assert.Equal(typeof(int), typeof(List<int>).GetItemTypeFromArrayOrListOfT());
            Assert.Equal(typeof(string), typeof(List<string>).GetItemTypeFromArrayOrListOfT());
            Assert.Equal(typeof(object), typeof(List<object>).GetItemTypeFromArrayOrListOfT());
            Assert.Equal(typeof(string), typeof(CustomStringList).GetItemTypeFromArrayOrListOfT());
            Assert.Equal(typeof(Dto), typeof(List<Dto>).GetItemTypeFromArrayOrListOfT());

            Assert.Equal(typeof(string), typeof(string[]).GetItemTypeFromArrayOrListOfT());      
            Assert.Equal(typeof(int), typeof(int[]).GetItemTypeFromArrayOrListOfT());  
            Assert.Equal(typeof(Dto), typeof(Dto[]).GetItemTypeFromArrayOrListOfT());  

            Assert.Null(typeof(string).GetItemTypeFromListOfT());
            Assert.Null(typeof(int).GetItemTypeFromListOfT());
            Assert.Null(typeof(Dictionary<string, int>).GetItemTypeFromListOfT());
        }

        [Fact]
        public void GetAllElementPropertyValues_ReturnsValuesFromListOfT()
        {
            var list = new List<Dto>()
            {
                new Dto{Name = "A"},
                new Dto{Name = "B"}
            };

            var values = list.GetAllElementPropertyValues(typeof(Dto).GetProperty("Name"));

            Assert.Equal(2, values.Length);
            Assert.Equal("A", values.GetValue(0));
            Assert.Equal("B", values.GetValue(1));

        }

        [Fact]
        public void GetAllElementPropertyValues_ReturnsValuesFromArray()
        {
            var array = new []
            {
                new Dto{Name = "A"},
                new Dto{Name = "B"}
            };

            var values = array.GetAllElementPropertyValues(typeof(Dto).GetProperty("Name"));

            Assert.Equal(2, values.Length);
            Assert.Equal("A", values.GetValue(0));
            Assert.Equal("B", values.GetValue(1));

        }

    }
}
