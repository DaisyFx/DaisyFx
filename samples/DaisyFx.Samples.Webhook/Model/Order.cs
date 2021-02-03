using System;

namespace DaisyFx.Samples.Webhook.Model
{
    public class Order
    {
        public DateTime OrderDate { get; set; }
        public OrderLine[] OrderLines { get; set; } = Array.Empty<OrderLine>();
    }
}