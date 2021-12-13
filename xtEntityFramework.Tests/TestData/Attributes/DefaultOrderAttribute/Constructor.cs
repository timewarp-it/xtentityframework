using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using xtEntityFramework.Attributes;
using xtEntityFramework.Models;

namespace xtEntityFramework.Tests.TestData.Attributes.DefaultOrderAttribute
{
    public class Constructor : IEnumerable<object[]>
    {
        private readonly List<object[]> _items = new List<object[]>()
        {
            new object[]
            {
                null,
                null,
            },
            new object[]
            {
                null,
                false
            },
            new object[]
            {
                null,
                true
            },
            new object[]
            {
                Order.Ascending,
                null,
            },
            new object[]
            {
                Order.Ascending,
                false
            },
            new object[]
            {
                Order.Ascending,
                true
            },
            new object[]
            {
                Order.Descending,
                null,
            },
            new object[]
            {
                Order.Descending,
                false
            },
            new object[]
            {
                Order.Descending,
                true
            }
        };

public IEnumerator<object[]> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
