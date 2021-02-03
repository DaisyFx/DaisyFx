using System;

namespace DaisyFx.Events.Source
{
    public readonly struct SourceStartedEvent : IDaisyEvent
    {
        public SourceStartedEvent(string chainName,
            string sourceName,
            int sourceIndex,
            in Guid sourceExecutionId)
        {
            ChainName = chainName;
            SourceName = sourceName;
            SourceIndex = sourceIndex;
            SourceExecutionId = sourceExecutionId;
        }

        public string ChainName { get; }
        public string SourceName { get; }
        public int SourceIndex { get; }
        public Guid SourceExecutionId { get; }
    }
}