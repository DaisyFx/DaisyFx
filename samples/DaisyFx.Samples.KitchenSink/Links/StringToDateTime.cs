using System;
using System.Globalization;
using System.Threading.Tasks;

namespace DaisyFx.Samples.KitchenSink.Links
{
    public class StringToDateTime : StatefulLink<string, DateTime>
    {
        protected override ValueTask<DateTime> Invoke(string input, ChainContext context)
        {
            return new(DateTime.Parse(input, CultureInfo.InvariantCulture));
        }
    }
}