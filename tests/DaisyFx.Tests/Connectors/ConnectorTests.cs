using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DaisyFx.Tests.Utils.Chains;
using DaisyFx.Tests.Utils.Extensions;
using DaisyFx.Tests.Utils.Links;
using Xunit;

namespace DaisyFx.Tests.Connectors
{
    public class ConnectorTests
    {
        [Fact]
        public async Task ConfigureConnector_MultipleTimes_Throws()
        {
            var chainBuilder = new TestChain<Signal>
            {
                ConfigureRootAction = root =>
                {
                    var builder = root.Link<NoopLink<Signal>, Signal>();
                    builder.Link<NoopLink<Signal>, Signal>();
                    builder.Link<NoopLink<Signal>, Signal>();
                }
            };

            await Assert.ThrowsAsync<NotSupportedException>(async () => await chainBuilder.BuildAsync());
        }

        [Fact]
        public async Task Init_ConnectorIndexIsSet()
        {
            var indexes = new List<(int expected, int actual)>();

            var chainBuilder = new TestChain<Signal>
            {
                ConfigureRootAction = root => root
                    .TestInspect(onInit: s => indexes.Add((0, s.Index)))
                    .SubChain(subChain => subChain
                        .TestInspect(onInit: s => indexes.Add((2, s.Index)))
                    )
                    .TestInspect(onInit: s => indexes.Add((3, s.Index)))
            };

            await chainBuilder.BuildAsync();

            Assert.Equal(3, indexes.Count);
            Assert.Equal(
                indexes.Select(d => d.expected),
                indexes.Select(d => d.actual)
            );
        }

        [Fact]
        public async Task Init_NestedConnector_HasIncreasedDepth()
        {
            var depths = new List<(int expected, int actual)>();

            var chainBuilder = new TestChain<Signal>
            {
                ConfigureRootAction = root => root
                    .TestInspect(onInit: s => depths.Add((0, s.Depth)))
                    .SubChain(firstSubChain => firstSubChain
                        .TestInspect(onInit: s => depths.Add((1, s.Depth)))
                        .SubChain(secondSubChain => secondSubChain
                            .TestInspect(onInit: s => depths.Add((2, s.Depth)))
                            .TestInspect(onInit: s => depths.Add((2, s.Depth)))
                        )
                        .TestInspect(onInit: s => depths.Add((1, s.Depth)))
                    )
                    .TestInspect(onInit: s => depths.Add((0, s.Depth)))
            };

            await chainBuilder.BuildAsync();

            Assert.Equal(
                depths.Select(d => d.expected),
                depths.Select(d => d.actual)
            );
        }

        [Fact]
        public async Task Process_ReceivesInput()
        {
            const string payload = "Test";
            var result = new List<string>();

            var chainBuilder = new TestChain<string>
            {
                ConfigureRootAction = root =>
                    root.TestInspect(onProcess: (input, _) => result.Add(input))
            };

            await chainBuilder.BuildAndExecuteAsync(payload);

            Assert.Single(result, payload);
        }
    }
}