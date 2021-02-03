using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DaisyFx.Samples.KitchenSink.Links
{
    public class LogDateTime : StatefulLink<DateTime, DateTime>
    {
        private readonly LogDateTimeConfiguration _configuration;

        public LogDateTime()
        {
            _configuration = ReadConfiguration<LogDateTimeConfiguration>();
        }

        protected override ValueTask<DateTime> Invoke(DateTime input, ChainContext context)
        {
            context.Logger.LogInformation(input.ToString(_configuration.DateFormat));
            return new ValueTask<DateTime>();
        }
    }

    public class LogDateTimeConfiguration
    {
        public string DateFormat { get; set; } = "yyyy-MM-dd";
    }
}