using System.Threading;
using System.Threading.Tasks;

namespace DaisyFx.Tests.Utils.Sources
{
    public class InfiniteDelaySource : SignalTestSource
    {
        public override async Task ExecuteAsync(SourceNextDelegate<Signal> next, CancellationToken cancellationToken)
        {
            await Task.Delay(Timeout.Infinite, cancellationToken);
            await base.ExecuteAsync(next, cancellationToken);
        }
    }
}