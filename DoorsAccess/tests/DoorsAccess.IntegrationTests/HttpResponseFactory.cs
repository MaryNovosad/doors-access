using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DoorsAccess.IntegrationTests;

public class HttpResponseFactory
{
    public static async Task<HttpResponse<T>> CreateAsync<T>(HttpResponseMessage message)
    {
        var jsonSerializationOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            WriteIndented = true
        };

        jsonSerializationOptions.Converters.Add(new JsonStringEnumConverter());

        var response = new HttpResponse<T>
        {
            StatusCode = message.StatusCode,
            Result = message.IsSuccessStatusCode ? await message.Content.ReadFromJsonAsync<T?>(jsonSerializationOptions) : default
        };

        return response;
    }
}