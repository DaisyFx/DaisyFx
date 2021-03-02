using System;
using Microsoft.Extensions.Logging;

namespace DaisyFx.Logging
{
    internal static class ChainLogMessages
    {
        private static readonly Action<ILogger, Exception?> RequestingLockMessage;
        private static readonly Action<ILogger, Exception?> ExecutingMessage;
        private static readonly Action<ILogger, Exception?> ExecutionCompletedMessage;
        private static readonly Action<ILogger, Exception?> ExecutionFaultedMessage;

        static ChainLogMessages()
        {
            RequestingLockMessage = LoggerMessage.Define(
                LogLevel.Trace,
                new EventId(1, nameof(RequestingLockMessage)),
                "Requesting lock");

            ExecutingMessage = LoggerMessage.Define(
                LogLevel.Trace,
                new EventId(2, nameof(ExecutingMessage)),
                "Executing");

            ExecutionCompletedMessage = LoggerMessage.Define(
                LogLevel.Information,
                new EventId(3, nameof(ExecutionCompletedMessage)),
                "Execution completed");

            ExecutionFaultedMessage = LoggerMessage.Define(
                LogLevel.Error,
                new EventId(4, nameof(ExecutionFaultedMessage)),
                "Execution faulted");
        }

        public static void RequestingLock(this ILogger<IChain> logger)
            => RequestingLockMessage(logger, null);

        public static void Executing(this ILogger<IChain> logger)
            => ExecutingMessage(logger, null);

        public static void ExecutionCompleted(this ILogger<IChain> logger)
            => ExecutionCompletedMessage(logger, null);

        public static void ExecutionFaulted(this ILogger<IChain> logger, Exception exception)
            => ExecutionFaultedMessage(logger, exception);
    }
}