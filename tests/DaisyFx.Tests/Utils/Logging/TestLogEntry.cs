using System;
using Microsoft.Extensions.Logging;

namespace DaisyFx.Tests.Utils.Logging
{
    public class TestLogEntry
    {
        public LogLevel LogLevel { get; }
        public EventId EventId { get; }
        public Exception Exception { get; }
        public string Message { get; }

        public TestLogEntry(LogLevel logLevel, EventId eventId, Exception exception, string message)
        {
            LogLevel = logLevel;
            EventId = eventId;
            Exception = exception;
            Message = message;
        }
    }
}