using DaisyFx.Sources.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Builder
{
    // ReSharper disable CheckNamespace
    // ReSharper disable UnusedType.Global
    // ReSharper disable MemberCanBePrivate.Global
    public static class EndpointRouteBuilderExtensions
    {
        public static IEndpointConventionBuilder MapDaisy(this IEndpointRouteBuilder builder, string routePrefix = "daisy")
        {
            return builder.MapPost($"{routePrefix}/{{chainName}}", async context =>
            {
                if (context.Request.RouteValues.TryGetValue("chainName", out var chainName) &&
                    chainName is string chainNameString)
                {
                    var router = context.RequestServices.GetRequiredService<HttpChainRouter>();
                    await router.Route(context, chainNameString);
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                }
            });
        }
    }
}
