namespace DaisyFx
{
    public class ExecutionResult
    {
        public static readonly ExecutionResult Completed = new(ExecutionResultStatus.Completed);

        private ExecutionResult(ExecutionResultStatus status, ChainException? exception = null)
        {
            Status = status;
            Exception = exception;
        }

        public ExecutionResultStatus Status { get; }
        public ChainException? Exception { get; }

        public static ExecutionResult Faulted(ChainException exception)
        {
            return new(ExecutionResultStatus.Faulted, exception);
        }
    }
}