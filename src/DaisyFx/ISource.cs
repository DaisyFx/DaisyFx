using System;
using System.Threading;
using System.Threading.Tasks;

namespace DaisyFx
{
    public interface ISource<out TOutput> : IDisposable
    {
        Task ExecuteAsync(
            SourceNextDelegate<TOutput> next,
            CancellationToken cancellationToken);
    }
}