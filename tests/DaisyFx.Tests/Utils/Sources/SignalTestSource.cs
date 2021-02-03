using System.Threading;
using System.Threading.Tasks;

namespace DaisyFx.Tests.Utils.Sources
{
    public class SignalTestSource : Source<Signal>
    {
        private readonly TestExecutionTracker? _tracker;

        public SignalTestSource(TestExecutionTracker? tracker = null)
        {
            _tracker = tracker;
        }

        public override async Task ExecuteAsync(SourceNextDelegate<Signal> next, CancellationToken cancellationToken)
        {
            await next(Signal.Static);
        }

        protected override void Dispose()
        {
            _tracker?.SourceDisposed(Name);
            base.Dispose();
        }
    }
}