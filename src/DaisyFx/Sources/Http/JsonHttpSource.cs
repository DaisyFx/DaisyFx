using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DaisyFx.Sources.Http
{
    public class JsonHttpSource<T> : HttpSource<T>
    {
        protected JsonSerializerOptions SerializerOptions { get; }

        public JsonHttpSource(HttpChainRouter router) : base(router)
        {
            SerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        }

        protected sealed override ValueTask<T?> DeserializePayload(HttpRequest httpRequest)
        {
            return httpRequest.ReadFromJsonAsync<T>(SerializerOptions);
        }
    }
}