using FclEx.Utils;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace FclEx.Fw.Test.Logging
{
    public class TestLoggerProvider : ILoggerProvider
    {
        private readonly ITestOutputHelper _output;
        private readonly LazyLock<TestLogger> _lazy = new LazyLock<TestLogger>();

        public TestLoggerProvider(ITestOutputHelper output)
        {
            _output = output;
        }

        public void Dispose()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _lazy.Get(() => new TestLogger(_output));
        }
    }
}
