using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Integration.Test.Host
{
    public class TestHttpClient
    {
        protected readonly HttpClient client;

        public TestHttpClient(HttpClient httpClient)
        {
            this.client = httpClient;
        }

        public async Task<HttpResponseMessage> ExecuteHttpRequestAsync(HttpRequestConfiguration configuration, CancellationToken cancellationToken = default) =>
            configuration.HttpMethod.Method switch
            {
                "GET" => await client.DoGetRequestAsync(configuration, cancellationToken),
                "POST" => await client.DoPostRequestAsync(configuration, cancellationToken),
                "PUT" => await client.DoPutRequestAsync(configuration, cancellationToken),
                "PATCH" => await client.DoPatchRequestAsync(configuration, cancellationToken),
                "DELETE" => await client.DoDeleteRequestAsync(configuration, cancellationToken),
                _ => throw new ArgumentException($"{configuration.HttpMethod.Method} is not a implemented HTTP call method.", nameof(configuration.HttpMethod))
            };
    }
}
