using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace FclEx.Logger
{
    public static class Extensions
    {
        public static ILoggerFactory AddEmpty(this ILoggerFactory factory)
        {
            factory.AddProvider(EmptyLoggerProvider.Instance);
            return factory;
        }

        public static ILoggerFactory AddSimpleConsole(this ILoggerFactory factory)
        {
            factory.AddProvider(new SimpleConsoleLoggerProvider());
            return factory;
        }
    }
}
