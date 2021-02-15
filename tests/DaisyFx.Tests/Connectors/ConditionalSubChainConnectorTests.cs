using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DaisyFx.Tests.Utils.Chains;
using DaisyFx.Tests.Utils.Extensions;
using DaisyFx.Tests.Utils.Links;
using Xunit;

namespace DaisyFx.Tests.Connectors
{
    public class ConditionalSubChainConnectorTests
    {
        [Fact]
        public async Task Process_CallsSubChainOnMatch()
        {
            const string payload1 = "Payload1";
            const string payload2 = "Payload2";
            const string payload3 = "Payload3";

            var result1 = new List<string>();
            var result2 = new List<string>();

            var chainBuilder = new TestChain<string>
            {
                ConfigureRootAction = root => root
                    .If(payload1.Equals, then => then
                        .TestInspect(onProcess: (input, _) => result1.Add(input))
                    )
                    .If(payload2.Equals, then => then
                        .TestInspect(onProcess: (input, _) => result2.Add(input))
                    )
            };

            using var chain = await chainBuilder.BuildAsync();

            await chain.ExecuteAsync(payload1, CancellationToken.None);
            await chain.ExecuteAsync(payload2, CancellationToken.None);
            await chain.ExecuteAsync(payload3, CancellationToken.None);

            Assert.Single(result1);
            Assert.Single(result1, payload1);
            Assert.Single(result2);
            Assert.Single(result2, payload2);
        }

        [Fact]
        public async Task Process_ReturnsInput()
        {
            const string payload = "Test";

            var result = new List<string>();

            var chainBuilder = new TestChain<string>
            {
                ConfigureRootAction = root => root
                    .If(payload.Equals, then => then
                        .Link<NoopLink<string>, string>()
                    )
                    .TestInspect(onProcess: (input, _) => result.Add(input))
            };

            await chainBuilder.BuildAndExecuteAsync(payload);

            Assert.Single(result, payload);
        }

        [Fact]
        public async Task Dispose_DisposesSubChain()
        {
            var subChainDisposed = false;

            var chainBuilder = new TestChain<Signal>
            {
                ConfigureRootAction = root => root
                    .If(_ => true, then => then
                        .TestInspect(onDispose: () => subChainDisposed = true)
                    )
            };

            var chain = await chainBuilder.BuildAsync();
            chain.Dispose();

            Assert.True(subChainDisposed);
        }
    }
}