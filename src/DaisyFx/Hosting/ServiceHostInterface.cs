using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DaisyFx.Hosting
{
    internal class ServiceHostInterface : IHostInterface
    {
        private readonly IChain[] _chains;

        public ServiceHostInterface(IEnumerable<IChain> chains)
        {
            _chains = chains.ToArray();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var chain in _chains)
            {
                await chain.InitAsync(cancellationToken);
            }

            foreach (var chain in _chains)
            {
                chain.StartAllSources();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            var chainStopTasks = new Task[_chains.Length];

            for (var i = 0; i < chainStopTasks.Length; i++)
            {
                chainStopTasks[i] = _chains[i].StopAllSourcesAsync(true);
            }

            return Task.WhenAny(
                Task.WhenAll(chainStopTasks),
                Task.Delay(Timeout.Infinite, cancellationToken));
        }
    }
}