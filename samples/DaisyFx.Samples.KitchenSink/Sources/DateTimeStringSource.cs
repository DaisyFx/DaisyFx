using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace DaisyFx.Samples.KitchenSink.Sources
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DateTimeStringSource : Source<string>
    {
        public override Task ExecuteAsync(SourceNextDelegate<string> next, CancellationToken cancellationToken)
        {
            return next(DateTime.Now.ToString(CultureInfo.InvariantCulture));
        }
    }
}