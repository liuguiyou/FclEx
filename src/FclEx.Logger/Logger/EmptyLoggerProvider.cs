using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace FclEx.Logger
{
    public class EmptyLoggerProvider : ILoggerProvider
    {
        public static EmptyLoggerProvider Instance { get; } = new EmptyLoggerProvider();

        public void Dispose()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            return EmptyLogger.Instance;
        }
    }
}
