using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace xtEntityFramework.Tests.TestData.PropertyCache
{
    public class GetProperty : IEnumerable<object[]>
    {
        private readonly List<object[]> _items = new List<object[]>()
        {
            new object[]
            {
                typeof (string),
                "Length"
            },
            new object[]
            {
                typeof (string),
                "Count"
            },
            new object[]
            {
                typeof (int),
                "Count"
            }
        };

        public IEnumerator<object[]> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class GetPropertyWithExpected : IEnumerable<object[]>
    {
        private readonly List<object[]> _items = new List<object[]>()
        {
            new object[]
            {
                typeof(string),
                "Length",
                typeof(string).GetProperty(
                    "Length",
                    BindingFlags.Static |
                    BindingFlags.FlattenHierarchy |
                    BindingFlags.Instance |
                    BindingFlags.Public)
            },
            new object[]
            {
                typeof(string),
                "Count",
                typeof(string).GetProperty(
                    "Count",
                    BindingFlags.Static |
                    BindingFlags.FlattenHierarchy |
                    BindingFlags.Instance |
                    BindingFlags.Public)
            },
            new object[]
            {
                typeof(int),
                "Count",
                typeof(int).GetProperty(
                    "Count",
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
