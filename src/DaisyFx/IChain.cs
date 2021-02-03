using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DaisyFx.Connectors;
using DaisyFx.Sources;

namespace DaisyFx
{
    public interface IChain : IDisposable
    {
        string Name { get; }
        IReadOnlyList<IConnector> Connectors { get; }
        IReadOnlyList<ISourceConnector> Sources { get; }
        ValueTask InitAsync(CancellationToken cancellationToken);
        void StartAllSources();
        Task StopAllSourcesAsync(bool force = false);
        void StartSource(int sourceIndex);
        Task StopSourceAsync(int sourceIndex, bool force = false);
    }
}