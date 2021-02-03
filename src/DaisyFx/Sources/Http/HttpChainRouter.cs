using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DaisyFx.Sources.Http
{
    public class HttpChainRouter
    {
        private readonly ConcurrentDictionary<string, Func<HttpContext, Task>> _handlers =
            new(StringComparer.OrdinalIgnoreCase);

        internal void AddRoute(string chainName, Func<HttpContext, Task> routeHandler)
        {
            _handlers.TryAdd(chainName, routeHandler);
        }

        internal void RemoveRoute(string chainName)
        {
            _handlers.TryRemove(chainName, out _);
        }

        public Task Route(HttpContext context, string chainName)
        {
            if (_handlers.TryGetValue(chainName, out var handler))
            {
                return handler(context);
            }

            context.Response.StatusCode = StatusCodes.Status404NotFound;
            return Task.CompletedTask;
        }
    }
}