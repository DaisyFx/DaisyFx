using System;

namespace DaisyFx.Events.Chain
{
    public readonly struct ChainExecutionResultEvent : IDaisyEvent
    {
        public ChainExecutionResultEvent(IReadOnlyChainContext context,
            TimeSpan duration,
            ExecutionResult result)
        {
            Context = context;
            Duration = duration;
            Result = result;
        }

        public IReadOnlyChainContext Context { get; }
        public TimeSpan Duration { get; }
        public ExecutionResult Result { get; }
    }
}