using System.Threading;
using System.Threading.Tasks;

namespace DaisyFx.Sources
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class TriggerOnStartupSource : Source<Signal>
    {
        public override Task ExecuteAsync(SourceNextDelegate<Signal> next, CancellationToken cancellationToken)
        {
            return next(Signal.Static);
        }
    }
}