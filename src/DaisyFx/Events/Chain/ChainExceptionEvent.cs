using System;

namespace DaisyFx.Events.Chain
{
    public class ChainExceptionEvent : IDaisyEventAsync
    {
        public ChainExceptionEvent(IReadOnlyChainContext context, Exception exception)
        {
            Context = context;
            Exception = exception;
        }

        public IReadOnlyChainContext Context { get; }
        public Exception Exception { get; }
    }
}