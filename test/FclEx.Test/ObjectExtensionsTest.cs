using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace FclEx.Test
{
    public class ObjectExtensionsTest
    {
        [Fact]
        public void CastTest()
        {
            var i = 5;
            var actual = i.CastTo<double>();
            var expected = (double) i;
            Assert.Equal(expected, actual);
        }
    }
}
