using Microsoft.Extensions.Logging;

namespace DaisyFx.Tests.Utils.Logging
{
    public class TestLoggerProvider : ILoggerProvider
    {
        private readonly TestLogSink _logSink;

        public TestLoggerProvider(TestLogSink testLogSink)
        {
            _logSink = testLogSink;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new TestLogger(_logSink);
        }

        public void Dispose()
        {
        }
    }
}