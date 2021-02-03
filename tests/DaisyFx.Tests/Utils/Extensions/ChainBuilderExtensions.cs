using System.Threading;
using System.Threading.Tasks;

namespace DaisyFx.Tests.Utils.Extensions
{
    public static class ChainBuilderExtensions
    {
        public static async Task<Chain<T>> BuildAsync<T>(this ChainBuilder<T> chainBuilder,
            TestServiceProvider? serviceProvider = null)
        {
            var chain = ((IChainBuilder)chainBuilder).BuildChain(serviceProvider ?? new TestServiceProvider());
            await chain.InitAsync(default);

            return (Chain<T>) chain;
        }

        public static async Task<ExecutionResult> BuildAndExecuteAsync<T>(this ChainBuilder<T> chainBuilder,
            T input,
            CancellationToken cancellationToken = default,
            TestServiceProvider? serviceProvider = null)
        {
            using var chain = await BuildAsync(chainBuilder, serviceProvider);
            return await chain.ExecuteAsync(input, cancellationToken);
        }
    }
}