using System.Threading;
using System.Threading.Tasks;

namespace DaisyFx.TestHelpers
{
    public class StatelessLinkTestRunner<TLink, TInput, TOutput> : LinkTestRunnerBase<TLink, TInput, TOutput>
        where TLink : StatelessLink<TInput, TOutput>
    {
        public async ValueTask<TOutput> ExecuteAsync(TInput input, CancellationToken ct = default)
        {
            await using var context = CreateContext(Services, ct);
            using ILink<TInput, TOutput> link = CreateLink(context.ScopeServices);

            return await link.ExecuteAsync(input, context);
        }
    }
}