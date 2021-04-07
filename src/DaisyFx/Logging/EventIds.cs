using Microsoft.Extensions.Logging;

namespace DaisyFx.Logging
{
    internal static class EventIds
    {
        // Chain - 1xxx
        public static readonly EventId RequestingLockMessage = new(1000, nameof(RequestingLockMessage));
        public static readonly EventId ExecutingMessage = new(1001, nameof(ExecutingMessage));
        public static readonly EventId ExecutionCompletedMessage = new(1002, nameof(ExecutionCompletedMessage));
        public static readonly EventId ExecutionFaultedMessage = new(1003, nameof(ExecutionFaultedMessage));

        // Source - 2xxx
        public static readonly EventId SourceCompletedMessage = new(2000, nameof(SourceCompletedMessage));
        public static readonly EventId SourceCanceledMessage = new(2001, nameof(SourceCanceledMessage));
        public static readonly EventId SourceFatalErrorMessage = new(2002, nameof(SourceFatalErrorMessage));
    }
}