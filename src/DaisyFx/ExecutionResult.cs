using System;

namespace DaisyFx
{
    public class ExecutionResult
    {
        public static readonly ExecutionResult Completed = new(ExecutionResultStatus.Completed);

        private ExecutionResult(ExecutionResultStatus status, Exception? exception = null)
        {
            Status = status;
            Exception = exception;
        }

        public ExecutionResultStatus Status { get; }
        public Exception? Exception { get; }

        public static ExecutionResult Faulted(Exception exception)
        {
            return new(ExecutionResultStatus.Faulted, exception);
        }
    }
}