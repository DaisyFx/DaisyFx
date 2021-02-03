using System.Collections.Concurrent;

namespace DaisyFx.Tests.Utils
{
    public class TestExecutionTracker
    {
        public ConcurrentDictionary<string, int> DisposeCountBySourceName { get; set; } = new();

        public void SourceDisposed(string name)
        {
            DisposeCountBySourceName.AddOrUpdate(name, _ => 1, (_, i) => ++i);
        }
    }
}