using System;
using System.Threading.Tasks;
using DaisyFx.Events.Connector;

namespace DaisyFx.Connectors
{
    internal abstract class Connector<TInput, TOutput> : IConnector<TInput, TOutput>
    {
        private IConnector<TOutput> _nextConnector = SinkConnector<TOutput>.Static;
        private readonly ConnectorContext _context;

        protected Connector(string name, ConnectorContext context)
        {
            Name = name;
            InputType = typeof(TInput);
            OutputType = typeof(TOutput);
            _context = context;
            (Index, Depth) = context.Add(this);
        }

        public int Index { get; }
        public int Depth { get; }
        public string Name { get; }
        public Type InputType { get; }
        public Type OutputType { get; }
        ConnectorContext IConnectorLinker<TOutput>.Context => _context;

        async ValueTask IConnector<TInput>.ProcessAsync(TInput input, ChainContext context)
        {
            TOutput result;
            var startTickCount = Environment.TickCount64;
            try
            {
                context.EventBroker.Publish(new ConnectorStartingEvent(context, this));
                context.CancellationToken.ThrowIfCancellationRequested();
                result = await ProcessAsync(input, context);
            }
            catch (Exception exception)
            {
                context.SetResult(ExecutionResult.Faulted, exception);
                return;
            }
            finally
            {
                var duration = TimeSpan.FromMilliseconds(Environment.TickCount64 - startTickCount);
                context.EventBroker.Publish(new ConnectorStoppedEvent(context, this, duration));
            }

            if (context.Result == ExecutionResult.Unknown)
            {
                await _nextConnector.ProcessAsync(result, context);
            }
        }

        protected abstract ValueTask<TOutput> ProcessAsync(TInput input, ChainContext context);

        IConnectorLinker<TNextValue> IConnectorLinker<TOutput>.Link<TNextValue>(IConnector<TOutput, TNextValue> connector)
        {
            if (_nextConnector != SinkConnector<TOutput>.Static)
            {
                throw new NotSupportedException("Continuation already configured");
            }

            _nextConnector = connector;
            return connector;
        }

        IConnectorLinker<TNextValue> IConnectorLinker<TOutput>.Link<TLink, TNextValue>(string? name)
        {
            if (_nextConnector != SinkConnector<TOutput>.Static)
            {
                throw new NotSupportedException("Continuation already configured");
            }

            var connector = LinkConnectorFactory.Create<TOutput, TLink, TNextValue>(name, _context);
            _nextConnector = connector;
            return connector;
        }

        void IDisposable.Dispose()
        {
            _nextConnector.Dispose();
            Dispose();
        }

        protected virtual void Dispose()
        {
        }
    }
}