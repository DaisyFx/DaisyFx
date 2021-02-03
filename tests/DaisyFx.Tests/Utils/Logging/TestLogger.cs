using System;
using Microsoft.Extensions.Logging;

namespace DaisyFx.Tests.Utils.Logging
{
    public class TestLogger : ILogger
    {
        private readonly TestLogSink _logSink;

        public TestLogger(TestLogSink logSink)
        {
            _logSink = logSink;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var message = formatter(state, exception);
            var logEntry = new TestLogEntry(logLevel, eventId, exception, message);
            _logSink.Log(logEntry);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return NullScope.Instance;
        }

        private class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new NullScope();

            public void Dispose()
            {
            }
        }
    }
}