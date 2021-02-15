using System;
using System.Globalization;
using DaisyFx.NCrontab;
using DaisyFx.Samples.KitchenSink.Links;
using DaisyFx.Samples.KitchenSink.Sources;

namespace DaisyFx.Samples.KitchenSink
{
    public class KitchenSinkChain : ChainBuilder<string>
    {
        public override string Name { get; } = "KitchenSink";

        public override void ConfigureSources(SourceConnectorCollection<string> sources)
        {
            sources.Add<DateTimeStringSource>("DateTimeStringSource");
            sources.Add<NCrontabSource, Signal>("Cron", _ => DateTime.Now.ToString(CultureInfo.InvariantCulture));
        }

        public override void ConfigureRootConnector(IConnectorLinker<string> root)
        {
            root.If(string.IsNullOrEmpty, _ => _
                    .Complete("Payload is empty"))
                .SubChain(subChain => subChain
                    .Link<GenerateArray, int[]>("GenerateArray")
                    .Each(each => each
                        .If(IsDivisibleBy3, then => then
                            .Map(_ => "Fizz")
                            .Link<LogInformation<string>, string>()
                        )
                        .If(IsDivisibleBy5, then => then
                            .Map(_ => "Buzz")
                            .Link<LogInformation<string>, string>()
                        )
                        .If(i => !IsDivisibleBy3(i) && !IsDivisibleBy5(i), then => then
                            .Link<LogInformation<int>, int>()
                        )))
                .Link<StringToDateTime, DateTime>()
                .Link<LogDateTime, DateTime>();
        }

        private static bool IsDivisibleBy3(int input)
        {
            return input % 3 == 0;
        }

        private static bool IsDivisibleBy5(int input)
        {
            return input % 5 == 0;
        }
    }
}