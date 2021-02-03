using System;

namespace DaisyFx.Events.Source
{
    public class SourceExceptionEvent : IDaisyEventAsync
    {
        public SourceExceptionEvent(string chainName,
            string sourceName,
            int sourceIndex,
            Guid sourceExecutionId,
            Exception exception)
        {
            ChainName = chainName;
            SourceName = sourceName;
            SourceIndex = sourceIndex;
            Exception = exception;
            SourceExecutionId = sourceExecutionId;
        }

        public string ChainName { get; }
        public string SourceName { get; }
        public int SourceIndex { get; }
        public Guid SourceExecutionId { get; }
        public Exception Exception { get; }
    }
}