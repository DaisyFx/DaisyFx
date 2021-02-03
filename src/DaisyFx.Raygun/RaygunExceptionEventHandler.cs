using System;
using System.Threading.Tasks;
using DaisyFx.Events;
using DaisyFx.Events.Chain;
using DaisyFx.Events.Source;
using Microsoft.Extensions.Options;
using Mindscape.Raygun4Net;

namespace DaisyFx.Raygun
{
    public class RaygunExceptionEventHandler : IDaisyExceptionEventHandler
    {
        private readonly IOptions<RaygunSettings> _raygunSettings;
        private readonly IRaygunClientProvider _raygunClientProvider;

        public RaygunExceptionEventHandler(IOptions<RaygunSettings> raygunSettings, IRaygunClientProvider raygunClientProvider)
        {
            _raygunSettings = raygunSettings;
            _raygunClientProvider = raygunClientProvider;
        }

        public async ValueTask HandleAsync(ChainExceptionEvent daisyEvent)
        {
            var payload = CreatePayload(daisyEvent);
            await Send(payload);
        }

        public async ValueTask HandleAsync(SourceExceptionEvent daisyEvent)
        {
            var payload = CreatePayload(daisyEvent);
            await Send(payload);
        }

        protected virtual RaygunPayload CreatePayload(ChainExceptionEvent daisyEvent)
        {
            var payload = new RaygunPayload(daisyEvent.Exception);

            payload.UserCustomData.Add("Chain", daisyEvent.Context.ChainName);

            return payload;
        }

        protected virtual RaygunPayload CreatePayload(SourceExceptionEvent daisyEvent)
        {
            var payload = new RaygunPayload(daisyEvent.Exception);

            payload.UserCustomData.Add("Chain", daisyEvent.ChainName);
            payload.UserCustomData.Add("Source", daisyEvent.SourceName);

            return payload;
        }

        protected virtual Task Send(RaygunPayload raygunPayload)
        {
            var client = _raygunClientProvider.GetClient(_raygunSettings.Value);
            return client.SendInBackground(raygunPayload.Exception, raygunPayload.Tags, raygunPayload.UserCustomData);
        }
    }
}