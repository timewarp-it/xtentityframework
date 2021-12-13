using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using SUT = xtEntityFramework;
using xtEntityFramework.Tests.TestData.Attributes.DefaultOrderAttribute;
using xtEntityFramework.Attributes;

namespace xtEntityFramework.Tests.Attributes.DefaultOrderAttribute
{
    public class Constructor
    {
        [Theory]
        [ClassData(typeof(Constructor))]
        public void BeEqualToGetPropertiesOfType(Order? order, bool? baseClass)
        {
            // arrange
            // act
            // assert
        }
    }
}
