using System;
using System.Threading;
using System.Threading.Tasks;

namespace DaisyFx.TestHelpers
{
    public class StatefulLinkTestRunner<TLink, TInput, TOutput>
        : LinkTestRunnerBase<TLink, TInput, TOutput>, IDisposable where TLink : StatefulLink<TInput, TOutput>
    {
        private readonly ILink<TInput, TOutput> _link;

        public StatefulLinkTestRunner(
            ConfigureServicesDelegate? services = null,
            (string key, string value)[]? configuration = null,
            string? chainName = null)
            : base(services, configuration, chainName)
        {
            _link = CreateLink(Services);
        }

        public async ValueTask<TOutput> ExecuteAsync(TInput input, CancellationToken ct = default)
        {
            using var context = CreateContext(Services, ct);
            return await _link.ExecuteAsync(input, context);
        }

        void IDisposable.Dispose()
        {
            _link.Dispose();
        }
    }
}