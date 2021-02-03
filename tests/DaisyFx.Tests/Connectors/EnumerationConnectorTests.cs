using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DaisyFx.Tests.Utils.Chains;
using DaisyFx.Tests.Utils.Extensions;
using Xunit;

namespace DaisyFx.Tests.Connectors
{
    public class EnumerationConnectorTests
    {
        [Fact]
        public async Task Process_StringArray_IteratesItems()
        {
            var payload = new[] {"Test1", "Test2", "Test3"};
            var result = new List<string>();

            var chainBuilder = new TestChain<string[]>
            {
                ConfigureRootAction = root => root
                    .Each(each => each
                        .TestInspect(onProcess: (input, _) => result.Add(input)))
            };

            await chainBuilder.BuildAndExecuteAsync(payload);

            Assert.True(payload.SequenceEqual(result));
        }

        [Fact]
        public async Task Cancel_CancellationToken_StopsIteration()
        {
            var payload = new[] {"Test1", "Test2", "Test3"};
            var result = new List<string>();
            var cancellationTokenSource = new CancellationTokenSource();
            const int maxIterations = 2;

            var chainBuilder = new TestChain<string[]>
            {
                ConfigureRootAction = root => root
                    .Each(each => each
                        .TestInspect(onProcess: (input, _) =>
                        {
                            result.Add(input);
                            if (result.Count == maxIterations)
                            {
                                cancellationTokenSource.Cancel();
                            }
                        }))
            };

            await chainBuilder.BuildAndExecuteAsync(payload, cancellationTokenSource.Token);

            Assert.True(payload.Take(maxIterations).SequenceEqual(result));
        }
    }
}