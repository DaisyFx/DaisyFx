using System;
using System.Threading.Tasks;

namespace DaisyFx.Connectors
{
    public interface IConnector
    {
        int Index { get; }
        int Depth { get; }
        string Name { get; }
        Type InputType { get; }
        Type OutputType { get; }
    }

    public interface IConnector<in TInput> : IConnector, IDisposable
    {
        internal ValueTask ProcessAsync(TInput input, ChainContext context);
    }

    public interface IConnector<in TInput, out TOutput> : IConnector<TInput>, IConnectorLinker<TOutput>
    {
        
    }
}