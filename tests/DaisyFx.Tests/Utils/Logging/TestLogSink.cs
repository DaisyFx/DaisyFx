using System.Collections.Concurrent;

namespace DaisyFx.Tests.Utils.Logging
{
    public class TestLogSink
    {
        private readonly ConcurrentQueue<TestLogEntry> _logEntries = new ConcurrentQueue<TestLogEntry>();

        public IProducerConsumerCollection<TestLogEntry> LogEntries => _logEntries;

        public void Log(TestLogEntry logEntry)
        {
            _logEntries.Enqueue(logEntry);
        }
    }
}