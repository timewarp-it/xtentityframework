using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using xtEntityFramework.Tests.TestData.PropertyCache;
using Xunit;
using SUT = xtEntityFramework;

namespace xtEntityFramework.Tests.PropertyCache
{
    public class GetProperty_Should
    {
        [Theory]
        [ClassData(typeof(GetProperty))]
        public void BeEqualToGetProperty<T>(T Type, string Name)
        {
            // arrange
            // act
            var result = SUT.PropertyCache.GetProperty<T>(Name);
            // assert
            result.Should().Equals(typeof(T).GetProperty(
                Name,
                BindingFlags.Static |
                BindingFlags.FlattenHierarchy |
                BindingFlags.Instance |
                BindingFlags.Public));
        }

        [Theory]
        [ClassData(typeof(GetPropertyWithExpected))]
        public void BeEqualToExpected<T>(T Type, string Name, PropertyInfo property)
        {
            // arrange
            // act
            var result = SUT.PropertyCache.GetProperty<T>(Name);
            // assert
            result.Should().Equals(property);
        }
    }
}
