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
    public class GetProperties_Should
    {
        [Theory]
        [ClassData(typeof(GetProperties))]
        public void BeEqualToGetPropertiesOfType<T>(T type)
        {
            // arrange
            // act
            var properties = SUT.PropertyCache.GetProperties<T>();
            // assert
            properties.Should().Equal(typeof(T).GetProperties(
                BindingFlags.Static |
                BindingFlags.FlattenHierarchy |
                BindingFlags.Instance |
                BindingFlags.Public));
        }

        [Theory]
        [ClassData(typeof(GetPropertiesWithExpected))]
        public void BeEqualToExpected<T>(T type, PropertyInfo[] properties)
        {
            // arrange
            // act
            var result = SUT.PropertyCache.GetProperties<T>();
            // assert
            result.Should().Equals(properties);
        }
    }
}
