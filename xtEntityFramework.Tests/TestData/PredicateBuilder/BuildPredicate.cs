using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using xtEntityFramework.Models;

namespace xtEntityFramework.Tests.TestData.PropertyCache
{
    public class BuildPredicate : IEnumerable<object[]>
    {
        private readonly List<object[]> _items = new List<object[]>()
        {
            new object[]
            {
                "",
                "Length",
                Comparison.Eq,
                "5"
            },
            new object[]
            {
                DateTime.UtcNow,
                "Day",
                Comparison.Gt,
                "5"
            }
        };

public IEnumerator<object[]> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
