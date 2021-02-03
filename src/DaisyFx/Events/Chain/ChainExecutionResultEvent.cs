using System;

namespace DaisyFx.Events.Chain
{
    public readonly struct ChainExecutionResultEvent : IDaisyEvent
    {
        public ChainExecutionResultEvent(IReadOnlyChainContext context,
            TimeSpan duration,
            ExecutionResult result,
            string? resultReason,
            Exception? exception)
        {
            Context = context;
            Duration = duration;
            Result = result;
            ResultReason = resultReason;
            Exception = exception;
        }

        public IReadOnlyChainContext Context { get; }
        public TimeSpan Duration { get; }
        public ExecutionResult Result { get; }
        public string? ResultReason { get; }
        public Exception? Exception { get; }
    }
}