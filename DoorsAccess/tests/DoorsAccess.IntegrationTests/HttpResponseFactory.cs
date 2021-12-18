using System.Net.Http.Json;

namespace DoorsAccess.IntegrationTests;

public class HttpResponseFactory
{
    public static async Task<HttpResponse<T>> CreateAsync<T>(HttpResponseMessage message)
    {
        var response = new HttpResponse<T>
        {
            StatusCode = message.StatusCode,
            Result = message.IsSuccessStatusCode ? await message.Content.ReadFromJsonAsync<T?>() : default
        };

        return response;
    }
}