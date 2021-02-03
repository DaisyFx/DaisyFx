using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DaisyFx.Sources.Http
{
    public abstract class HttpSource<T> : Source<T>
    {
        private readonly HttpChainRouter _router;

        protected HttpSource(HttpChainRouter router)
        {
            _router = router;
        }

        public sealed override async Task ExecuteAsync(SourceNextDelegate<T> next, CancellationToken cancellationToken)
        {
            var completion = new TaskCompletionSource();

            await using var cancellationRegistration = cancellationToken.Register(o =>
            {
                if (o is TaskCompletionSource tcs)
                {
                    tcs.SetResult();
                }
            }, completion);

            _router.AddRoute(ChainName, async httpContext =>
            {
                var request = httpContext.Request;
                if (!await Authorize(request))
                {
                    httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

                var payload = await DeserializePayload(request);
                if (payload is null)
                {
                    httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                    return;
                }

                if (!await ValidatePayload(request, payload))
                {
                    httpContext.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return;
                }

                httpContext.Response.StatusCode = StatusCodes.Status202Accepted;
                var _ = next(payload);
            });
            await completion.Task;
            _router.RemoveRoute(ChainName);
        }

        protected abstract ValueTask<T?> DeserializePayload(HttpRequest httpRequest);

        protected virtual ValueTask<bool> Authorize(HttpRequest httpRequest) => new(true);

        protected virtual ValueTask<bool> ValidatePayload(HttpRequest httpRequest, T payload) => new(true);
    }
}