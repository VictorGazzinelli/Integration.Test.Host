using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Integration.Test.Host
{
    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> DoGetRequestAsync(this HttpClient httpClient, HttpRequestConfiguration configuration, CancellationToken cancellationToken = default)
        {
            string requestUri = BuildRequestUriWithQueryParams(configuration.RequestUri, configuration.Parameters);

            return await httpClient.GetAsync(requestUri, cancellationToken);
        }

        public static async Task<HttpResponseMessage> DoPostRequestAsync(this HttpClient httpClient, HttpRequestConfiguration configuration, CancellationToken cancellationToken = default, string requestContentMediaType = MediaTypeNames.Application.Json)
        {
            HttpContent httpConent = BuildHttpContent(configuration.Parameters, requestContentMediaType);

            return await httpClient.PostAsync(configuration.RequestUri, httpConent, cancellationToken);
        }

        public static async Task<HttpResponseMessage> DoPutRequestAsync(this HttpClient httpClient, HttpRequestConfiguration configuration, CancellationToken cancellationToken = default, string requestContentMediaType = MediaTypeNames.Application.Json)
        {
            HttpContent httpConent = BuildHttpContent(configuration.Parameters, requestContentMediaType);

            return await httpClient.PutAsync(configuration.RequestUri, httpConent, cancellationToken);
        }

        public static async Task<HttpResponseMessage> DoPatchRequestAsync(this HttpClient httpClient, HttpRequestConfiguration configuration, CancellationToken cancellationToken = default, string requestContentMediaType = MediaTypeNames.Application.Json)
        {
            HttpContent httpConent = BuildHttpContent(configuration.Parameters, requestContentMediaType);

            return await httpClient.PutAsync(configuration.RequestUri, httpConent, cancellationToken);
        }

        public static async Task<HttpResponseMessage> DoDeleteRequestAsync(this HttpClient httpClient, HttpRequestConfiguration configuration, CancellationToken cancellationToken = default, string requestContentMediaType = MediaTypeNames.Application.Json)
        {
            string requestUri = BuildRequestUriWithQueryParams(configuration.RequestUri, configuration.Parameters);

            return await httpClient.DeleteAsync(requestUri, cancellationToken);
        }

        private static string BuildRequestUriWithQueryParams(string requestUri, object parameters) =>
            parameters != null ?
                $"{requestUri}?{BuildAsQueryString(parameters)}" :
                requestUri;

        private static string BuildAsQueryString(object parameters)
        {
            var parameterTypes = parameters.GetType().GetProperties().Select(x => x.PropertyType);

            if (parameters.GetType().GetProperties().All(x => x.PropertyType == typeof(string) || !typeof(IEnumerable).IsAssignableFrom(x.PropertyType)))
                return string.Join("&", parameters.GetType().GetProperties().Select(x => x.Name + "=" + WebUtility.UrlEncode(Convert.ToString(x.GetValue(parameters, null), CultureInfo.InvariantCulture))).ToArray());

            var stringParameters = parameters
                .GetType()
                .GetProperties()
                .Where(x => x.GetType() == typeof(string) || !typeof(IEnumerable).IsAssignableFrom(x.PropertyType))
                .Select(x => x.Name + "=" + WebUtility.UrlEncode(Convert.ToString(x.GetValue(parameters, null), CultureInfo.InvariantCulture)))
                .ToList();

            foreach (var enumerableProp in parameters.GetType().GetProperties().Where(x => typeof(IEnumerable).IsAssignableFrom(x.PropertyType) && x.PropertyType != typeof(string)))
            {
                IEnumerable enumerable = (IEnumerable)enumerableProp.GetValue(parameters, null);
                if (enumerable == null)
                    continue;
                int i = 0;
                foreach (var item in enumerable.Cast<object>())
                {
                    stringParameters.Add($"{enumerableProp.Name}={item}");
                    i++;
                }
            }

            return string.Join("&", stringParameters);
        }

        public static HttpContent BuildHttpContent(object parameters, string requestContentMediaType = MediaTypeNames.Application.Json)
        {
            if (parameters == null)
                return new StringContent("", Encoding.UTF8, requestContentMediaType);
            if (requestContentMediaType == MediaTypeNames.Application.Json)
                return new StringContent(JsonConvert.SerializeObject(parameters), Encoding.UTF8, requestContentMediaType);
            if (requestContentMediaType == "application/x-www-form-urlencoded")
                return new StringContent(BuildAsQueryString(parameters), Encoding.UTF8, requestContentMediaType);
            
            return new StringContent(parameters.ToString(), Encoding.UTF8, requestContentMediaType);
        }
    }
}
