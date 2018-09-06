using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace FclEx.Fw.Test
{
    public static class XunitExtensions
    {
        public static void ShouldBe<T>(this T obj, T value)
        {
            Assert.Equal(value, obj);
        }
    }
}
