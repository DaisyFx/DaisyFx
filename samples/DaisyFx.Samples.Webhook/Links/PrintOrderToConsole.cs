using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using DaisyFx.Samples.Webhook.Model;

namespace DaisyFx.Samples.Webhook.Links
{
    public class PrintOrderToConsole : StatefulLink<Order, Order>
    {
        protected override ValueTask<Order> ExecuteAsync(Order input, ChainContext context)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"OrderDate: {input.OrderDate.ToString(CultureInfo.InvariantCulture)}");
            builder.AppendLine($"OrderLines: {input.OrderLines.Length}");

            foreach (var orderLine in input.OrderLines)
            {
                builder.AppendLine($"    {orderLine.Article}: {orderLine.Quantity}");
            }

            Console.WriteLine(builder);

            return new(input);
        }
    }
}