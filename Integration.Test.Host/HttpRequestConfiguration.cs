using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;

namespace Integration.Test.Host
{
    public class HttpRequestConfiguration
    {
        public HttpMethod HttpMethod { get; set; }
        public string RequestUri { get; set; }
        public IDictionary<string, string> Headers { get; set; }
        public object Parameters { get; set; }
        public JsonSerializerSettings JsonSerializerSettings { get; set; }

        public HttpRequestConfiguration(HttpMethod httpMethod, string requestUri, object parameters = null, IDictionary<string, string> headers = null, JsonSerializerSettings jsonSerializerSettings = null)
        {
            HttpMethod = httpMethod;
            RequestUri = requestUri;
            Parameters = parameters;
            Headers = headers;
            JsonSerializerSettings = jsonSerializerSettings;
        }
    }
}