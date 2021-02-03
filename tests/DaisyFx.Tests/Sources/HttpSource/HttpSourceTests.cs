using System;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace DaisyFx.Tests.Sources.HttpSource
{
    public sealed class HttpSourceTests : IDisposable
    {
        private readonly IHost _testHost;

        public HttpSourceTests()
        {
            _testHost = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(x =>
                {
                    x.Configure(app =>
                    {
                        app.UseRouting();
                        app.UseAuthorization();
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapDaisy();
                        });
                    });
                    x.ConfigureServices((context, services) =>
                    {
                        services.AddAuthorization();
                        services.AddDaisy(context.Configuration, d =>
                        {
                            d.AddChain<HttpTestChain>();
                        });
                    });
                    x.UseTestServer();
                }).Start();
        }

        [Fact]
        public async Task Post_WithValidModelAndAuth_RespondsWithAccepted()
        {
            using var client = _testHost.GetTestClient();
            client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("ApiKey");
            using var result = await client.PostAsJsonAsync("/daisy/test", new HttpTestPayload
            {
                Name = "Daisy"
            });

            Assert.Equal(HttpStatusCode.Accepted, result.StatusCode);
        }

        [Fact]
        public async Task Post_WithoutRequiredAuth_RespondsWithUnauthorized()
        {
            using var client = _testHost.GetTestClient();
            using var result = await client.PostAsJsonAsync("/daisy/test", new HttpTestPayload
            {
                Name = "Daisy"
            });

            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Fact]
        public async Task Post_WithInvalidPayload_RespondsWithUnprocessableEntity()
        {
            using var client = _testHost.GetTestClient();
            client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("ApiKey");
            using var result = await client.PostAsJsonAsync("/daisy/test", new HttpTestPayload());

            Assert.Equal(HttpStatusCode.UnprocessableEntity, result.StatusCode);
        }

        public void Dispose()
        {
            _testHost.Dispose();
        }
    }
}