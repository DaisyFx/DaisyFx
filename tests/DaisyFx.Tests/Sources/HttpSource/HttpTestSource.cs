using System.Threading.Tasks;
using DaisyFx.Sources.Http;
using Microsoft.AspNetCore.Http;

namespace DaisyFx.Tests.Sources.HttpSource
{
    public class HttpTestSource : JsonHttpSource<HttpTestPayload>
    {
        public HttpTestSource(HttpChainRouter router) : base(router)
        {
        }

        protected override ValueTask<bool> Authorize(HttpRequest httpRequest)
        {
            var isAuthorized = httpRequest.Headers.TryGetValue("Authorization", out var values) &&
                               values.Count == 1;

            return new(isAuthorized);
        }

        protected override ValueTask<bool> ValidatePayload(HttpRequest httpRequest, HttpTestPayload payload)
        {
            return new(payload.Name is not null);
        }
    }
}