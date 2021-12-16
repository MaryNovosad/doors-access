using System.Net.Http.Json;
using DoorsAccess.API.Requests;
using DoorsAccess.API.Responses;

namespace DoorsAccess.IntegrationTests
{
    public class DoorsAccessAPIProxy
    {
        public static async Task<HttpResponseMessage> CreateDoorAsync(HttpClient httpClient, CreateOrUpdateDoorRequest request)
        {
            return await httpClient.PutAsJsonAsync($"v1/doors/{request.DoorId}", request);
        }

        public static async Task<HttpResponseMessage> AllowDoorAccessAsync(HttpClient httpClient, long doorId, AllowDoorAccessRequest request)
        {
            return await httpClient.PutAsJsonAsync($"v1/doors/{doorId}/access", request);
        }

        public static async Task<HttpResponseMessage> OpenDoorAsync(HttpClient httpClient, long doorId)
        {
            return await httpClient.PutAsync($"v1/doors/{doorId}/state/open", null);
        }

        public static async Task<HttpResponse<DoorsAccessHistoryResponse>> GetDoorAccessHistoryAsync(HttpClient httpClient, long userId)
        {
            var responseMessage = await httpClient.GetAsync($"v1/doors/history/user/{userId}");
            var getDoorHistoryResponse = await HttpResponseFactory.CreateAsync<DoorsAccessHistoryResponse>(responseMessage);

            return getDoorHistoryResponse;
        }

        public static async Task<HttpResponse<DoorsAccessHistoryResponse>> GetDoorAccessHistoryAsync(HttpClient httpClient)
        {
            var responseMessage = await httpClient.GetAsync("v1/doors/history");
            var getDoorHistoryResponse = await HttpResponseFactory.CreateAsync<DoorsAccessHistoryResponse>(responseMessage);

            return getDoorHistoryResponse;
        }

        public static async Task<HttpResponse<GetDoorResponse?>> GetDoorAsync(HttpClient httpClient, long doorId)
        {
            var responseMessage = await httpClient.GetAsync($"v1/doors/{doorId}");
            var getDoorResponse = await HttpResponseFactory.CreateAsync<GetDoorResponse?>(responseMessage);

            return getDoorResponse;
        }
    }
}
