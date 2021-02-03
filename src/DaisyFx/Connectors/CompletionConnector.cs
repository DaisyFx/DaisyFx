using System.Threading.Tasks;

namespace DaisyFx.Connectors
{
    internal class CompletionConnector<TInput> : Connector<TInput, TInput>
    {
        private readonly string _reason;

        public CompletionConnector(string reason, ConnectorContext context) : base("Completion", context)
        {
            _reason = reason;
        }

        protected override ValueTask<TInput> ProcessAsync(TInput input, ChainContext context)
        {
            context.SetResult(ExecutionResult.Completed, null, _reason);
            return new ValueTask<TInput>(input);
        }
    }
}