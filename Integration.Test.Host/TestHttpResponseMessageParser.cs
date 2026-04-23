using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Integration.Test.Host
{
    public static class TestHttpResponseMessageParser
    {
        public static async Task<T> ParseAsync<T>(HttpResponseMessage httpResponse, CancellationToken cancellationToken = default)
        {
            T response = default;
            string jsonPayload = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

            if (!jsonPayload.Equals(string.Empty))
            {
                try
                {
                    JsonSerializerOptions jsonSerializerOptionsIgnoreCase = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true, IncludeFields = true };
                    response = JsonSerializer.Deserialize<T>(jsonPayload, jsonSerializerOptionsIgnoreCase);
                }
                catch (Exception ex)
                {
                    string debugData = $"StatusCode: {httpResponse.StatusCode} - Response: {jsonPayload} - Exception: {ex.Message}";

                    if (typeof(T) == typeof(string))
                        return (T)Convert.ChangeType(debugData, typeof(T));
                    return default;
                }
            }

            return response;
        }
    }
}
