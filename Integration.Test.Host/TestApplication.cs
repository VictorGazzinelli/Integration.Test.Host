using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Options;
using Xunit;

namespace Integration.Test.Host
{
    public abstract class TestApplication<TProgram> where TProgram : class
    {
        protected TestApplicationFactory<TProgram> fixture;
        protected XunitLogger<TestApplication<TProgram>> logger;
        protected readonly HttpClient client;

        protected TestApplication(TestApplicationFactory<TProgram> fixture, ITestOutputHelper output)
        {
            this.fixture = fixture;
            this.fixture.SetTestOutputHelper(output);
            logger = new XunitLogger<TestApplication<TProgram>>(output);
            WebApplicationFactoryClientOptions options = new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = false
            };
            client = fixture.CreateClient(options);
        }

        protected T GetService<T>() => (T)fixture.Services.GetService(typeof(T));

        protected IOptions<T> GetServiceOptions<T>() where T : class => (IOptions<T>)fixture.Services.GetService(typeof(IOptions<T>));

        protected async Task<(HttpStatusCode statusCode, T response)> DoGetRequestAsync<T>(string requestUri, CancellationToken cancellationToken = default) =>
            await DoRequest<T>(HttpMethod.Get, requestUri, cancellationToken);

        private async Task<(HttpStatusCode statusCode, T response)> DoRequest<T>(HttpMethod httpMethod, string requestUri, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage httpResponse = await DoHttpRequest(httpMethod, requestUri, cancellationToken);

            T response = await ParseObjectResponseAsync<T>(httpResponse);

            return (httpResponse.StatusCode, response);
        }

        private async Task<HttpResponseMessage> DoHttpRequest(HttpMethod httpMethod, string requestUri, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage httpResponse = await ExecuteHttpRequest(client, httpMethod, requestUri, cancellationToken);

            return httpResponse;
        }

        private async Task<HttpResponseMessage> ExecuteHttpRequest(HttpClient client, HttpMethod httpMethod, string requestUri, CancellationToken cancellationToken = default) =>
            httpMethod.Method switch
            {
                "GET" => await client.GetAsync(requestUri, cancellationToken),
                _ => throw new ArgumentException($"{httpMethod.Method} is not a implemented HTTP call method.", nameof(httpMethod))
            };

        private async Task<T> ParseObjectResponseAsync<T>(HttpResponseMessage httpResponse)
        {
            T response = default;
            string responseContent = await httpResponse.Content.ReadAsStringAsync();

            if (typeof(T) == typeof(string))
                return (T)Convert.ChangeType(responseContent, typeof(T));

            return response;
        }
    }
}
