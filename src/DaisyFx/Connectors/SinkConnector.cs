using System;
using System.Threading.Tasks;

namespace DaisyFx.Connectors
{
    internal sealed class SinkConnector<T> : IConnector<T>
    {
        internal static readonly SinkConnector<T> Static = new();

        private SinkConnector()
        {
            Name = nameof(SinkConnector<T>);
            InputType = typeof(T);
            OutputType = typeof(void);
        }

        ValueTask IConnector<T>.ProcessAsync(T input, ChainContext context) => new();

        public int Index { get; } = -1;
        public int Depth { get; } = -1;
        public string Name { get; }
        public Type InputType { get; }
        public Type OutputType { get; }

        public void Dispose()
        {
        }
    }
}