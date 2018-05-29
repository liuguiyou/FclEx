using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace FclEx.Test
{
    public enum EnumTest
    {
        No = 0,
        Yes = 1,
    }

    public class ObjectExtensionsTests
    {
        [Fact]
        public void ObjectToIntCastTest()
        {
            object i = 5;
            var actual = i.CastTo<int>();
            var expected = (int)i;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IntToObjectCastTest()
        {
            var i = 5;
            var actual = i.CastTo<object>();
            var expected = (object)i;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void DoubleToIntCastTest()
        {
            var i = 5.0;
            var actual = i.CastTo<int>();
            var expected = (int)i;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IntToDoubleCastTest()
        {
            var i = 5;
            var actual = i.CastTo<double>();
            var expected = (double) i;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IntToEnumCastTest()
        {
            var i = 1;
            var actual = i.CastTo<EnumTest>();
            var expected = (EnumTest)i;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void EnumToIntCastTest()
        {
            var i = EnumTest.Yes;
            var actual = i.CastTo<int>();
            var expected = (int)i;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IntToNullableCastTest()
        {
            var i = 1;
            var actual = i.CastTo<int?>();
            var expected = (int?)i;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void NullableToIntCastTest()
        {
            int? i = 1;
            var actual = i.CastTo<int>();
            var expected = (int)i;
            Assert.Equal(expected, actual);
        }
    }
}
