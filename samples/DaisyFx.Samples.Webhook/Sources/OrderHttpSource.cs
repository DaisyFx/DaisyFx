using System;
using System.Linq;
using System.Threading.Tasks;
using DaisyFx.Samples.Webhook.Model;
using DaisyFx.Sources.Http;
using Microsoft.AspNetCore.Http;

namespace DaisyFx.Samples.Webhook.Sources
{
    public class OrderHttpSource : JsonHttpSource<Order>
    {
        private const string HardCodedApiKey = "SampleKey";

        public OrderHttpSource(HttpChainRouter router) : base(router)
        {
        }

        protected override ValueTask<bool> ValidatePayload(HttpRequest httpRequest, Order payload)
        {
            if (payload.OrderLines.Length <= 0)
                return new(false);

            return base.ValidatePayload(httpRequest, payload);
        }

        protected override ValueTask<bool> Authorize(HttpRequest httpRequest)
        {
            var isAuthorized = httpRequest.Headers.TryGetValue("Authorization", out var values) &&
                               values.Count == 1 && IsValidApiKey(values[0]);

            return new(isAuthorized);
        }

        private bool IsValidApiKey(string value)
        {
            return string.Equals($"bearer {HardCodedApiKey}", value, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}