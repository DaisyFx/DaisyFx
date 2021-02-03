using System;
using Microsoft.Extensions.Logging;

namespace DaisyFx.Logging
{
    internal static class ChainContextLogMessages
    {
        private static readonly Action<ILogger, string?, Exception?> RanToCompletionMessage;
        private static readonly Action<ILogger, string?, Exception?> ExecuteFailedMessage;

        static ChainContextLogMessages()
        {
            RanToCompletionMessage = LoggerMessage.Define<string?>(
                LogLevel.Information,
                new EventId(3, nameof(RanToCompletionMessage)),
                "Execute completed: {reason}");

            ExecuteFailedMessage = LoggerMessage.Define<string?>(
                LogLevel.Error,
                new EventId(5, nameof(ExecuteFailedMessage)),
                "Execute failed: {reason}");
        }

        public static void RanToCompletion(this ILogger<ChainContext> logger, string? reason, Exception? exception)
            => RanToCompletionMessage(logger, reason ?? string.Empty, exception);

        public static void Failed(this ILogger<ChainContext> logger, string? reason, Exception? exception)
            => ExecuteFailedMessage(logger, reason ?? string.Empty, exception);
    }
}