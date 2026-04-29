using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Xunit;

namespace Integration.Test.Host
{
    public abstract class TestApplication<TProgram> where TProgram : class
    {
        protected TestApplicationFactory<TProgram> fixture;
        protected XunitLogger<TestApplication<TProgram>> logger;
        protected readonly TestHttpClient client;

        protected TestApplication(TestApplicationFactory<TProgram> fixture, ITestOutputHelper output)
        {
            this.fixture = fixture;
            this.fixture.SetTestOutputHelper(output);
            this.logger = new XunitLogger<TestApplication<TProgram>>(output);
            this.client = new TestHttpClient(fixture.CreateClient());
        }

        protected T GetService<T>() => (T)fixture.Services.GetService(typeof(T));
        protected IOptions<T> GetServiceOptions<T>() where T : class => (IOptions<T>)fixture.Services.GetService(typeof(IOptions<T>));
        protected IOptionsSnapshot<T> GetServiceOptionsSnapshot<T>() where T : class => (IOptionsSnapshot<T>)fixture.Services.GetService(typeof(IOptionsSnapshot<T>));
        protected async Task<HttpStatusCode> DoRequest(HttpRequestConfiguration configuration, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage httpResponse = await client.ExecuteHttpRequestAsync(configuration, cancellationToken);

            return httpResponse.StatusCode;
        }

        protected async Task<(HttpStatusCode statusCode, T response)> DoRequest<T>(HttpRequestConfiguration configuration, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage httpResponse = await client.ExecuteHttpRequestAsync(configuration, cancellationToken);

            T response = await TestHttpResponseMessageParser.ParseAsync<T>(httpResponse);

            return (httpResponse.StatusCode, response);
        }   
    }
}
