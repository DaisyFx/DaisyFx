using System;
using DaisyFx.Sources;

namespace DaisyFx.Events.Source
{
    public readonly struct SourceStoppedEvent : IDaisyEvent
    {
        public SourceStoppedEvent(string chainName,
            string sourceName,
            int sourceIndex,
            in Guid sourceExecutionId,
            SourceResult result,
            Exception? exception = null)
        {
            ChainName = chainName;
            SourceName = sourceName;
            SourceIndex = sourceIndex;
            Result = result;
            SourceExecutionId = sourceExecutionId;
            Exception = exception;
        }

        public string ChainName { get; }
        public string SourceName { get; }
        public int SourceIndex { get; }
        public Guid SourceExecutionId { get; }
        public SourceResult Result { get; }
        public Exception? Exception { get; }
    }
}