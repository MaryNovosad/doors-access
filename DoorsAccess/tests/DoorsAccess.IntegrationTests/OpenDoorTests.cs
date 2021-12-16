using DoorsAccess.IntegrationTests.SetUp;
using NUnit.Framework;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using DoorsAccess.API.Requests;
using DoorsAccess.API.Responses;

namespace DoorsAccess.IntegrationTests
{
    public class OpenDoorTests : IntegrationTestBase
    {
        private const long TestDoorId = 1;

        [Test]
        public async Task Test1()
        {
            using var userHttpClient = CreateHttpClient();
            using var adminHttpClient = CreateHttpClient(TestAdminId, TestAdminRole);

            var createDoorsResponse = await CreateDoorAsync(adminHttpClient, new CreateOrUpdateDoorRequest
            {
                DoorId = TestDoorId,
                DoorName = "Clay office entrance door",
                IsDeactivated = false
            });

            var allowDoorAccessResponse = await AllowDoorAccess(adminHttpClient, TestDoorId, new AllowDoorAccessRequest
            {
                UsersIds = new List<long> { TestUserId }
            });

            var openDoorsResponse = await OpenDoorAsync(userHttpClient, TestDoorId);

            var userDoorAccessHistory = await GetDoorAccessHistory(userHttpClient, TestUserId);
            
            var doorsAccessHistoryResponse = await userDoorAccessHistory.Content.ReadFromJsonAsync<DoorsAccessHistoryResponse>();
        }

        private async Task<HttpResponseMessage> CreateDoorAsync(HttpClient httpClient, CreateOrUpdateDoorRequest request)
        {
            return await httpClient.PutAsJsonAsync($"v1/doors/{request.DoorId}", request);
        }

        private async Task<HttpResponseMessage> AllowDoorAccess(HttpClient httpClient, long doorId, AllowDoorAccessRequest request)
        {
            return await httpClient.PutAsJsonAsync($"v1/doors/{doorId}/access", request);
        }

        private async Task<HttpResponseMessage> OpenDoorAsync(HttpClient httpClient, long doorId)
        {
            return await httpClient.PutAsync($"v1/doors/{doorId}/state/open", null);
        }

        private async Task<HttpResponseMessage> GetDoorAccessHistory(HttpClient httpClient, long userId)
        {
            return await httpClient.GetAsync($"v1/doors/history/user/{userId}");
        }
    }
}