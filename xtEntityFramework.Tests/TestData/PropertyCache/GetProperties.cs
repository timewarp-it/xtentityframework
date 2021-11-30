using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace xtEntityFramework.Tests.TestData.PropertyCache
{
    public class GetProperties : IEnumerable<object[]>
    {
        private readonly List<object[]> _items = new List<object[]>()
        {
            new object[]
            {
                typeof(string)
            },
            new object[]
            {
                typeof(int)
            },
            new object[]
            {
                typeof(double)
            }
        };

        public IEnumerator<object[]> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class GetPropertiesWithExpected : IEnumerable<object[]>
    {
        private readonly List<object[]> _items = new List<object[]>()
        {
            new object[]
            {
                typeof(string),
                typeof(string).GetProperties(
                    BindingFlags.Static |
                    BindingFlags.FlattenHierarchy |
                    BindingFlags.Instance |
                    BindingFlags.Public)
            },
            new object[]
            {
                typeof(int),
                typeof(int).GetProperties(
                    BindingFlags.Static |
                    BindingFlags.FlattenHierarchy |
                    BindingFlags.Instance |
                    BindingFlags.Public)
            },
            new object[]
            {
                typeof(double),
                typeof(double).GetProperties(
                    BindingFlags.Static |
                    BindingFlags.FlattenHierarchy |
                    BindingFlags.Instance |
                    BindingFlags.Public)
            }
        };

        public IEnumerator<object[]> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
