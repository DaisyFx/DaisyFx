using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DaisyFx.Sources.Http
{
    public class SignalHttpSource : HttpSource<Signal>
    {
        public SignalHttpSource(HttpChainRouter router) : base(router)
        {
        }

        protected override ValueTask<Signal?> DeserializePayload(HttpRequest httpContext)
        {
            return new(Signal.Static);
        }
    }
}