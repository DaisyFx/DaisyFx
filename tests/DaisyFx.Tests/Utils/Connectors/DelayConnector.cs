using System.Threading.Tasks;
using DaisyFx.Connectors;

namespace DaisyFx.Tests.Utils.Connectors
{
    internal class DelayConnector<T> : Connector<T, T>
    {
        private readonly int _millisecondsDelay;

        public DelayConnector(int millisecondsDelay, ConnectorContext context) : base(nameof(DelayConnector<T>), context)
        {
            _millisecondsDelay = millisecondsDelay;
        }

        protected override async ValueTask<T> ProcessAsync(T input, ChainContext context)
        {
            await Task.Delay(_millisecondsDelay);
            return input;
        }
    }
}