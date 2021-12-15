using DoorsAccess.IntegrationTests.SetUp;
using NUnit.Framework;
using System.Net.Http.Headers;

namespace DoorsAccess.IntegrationTests
{
    public class OpenDoorTests : IntegrationTestBase
    {
        private const long TestDoorId = 1;

        [Test]
        public async Task Test1()
        {
            var userHttpClient = CreateHttpClient();
            var adminHttpClient = CreateHttpClient();

           var createDoorsResponse = await CreateDoorsAsync(adminHttpClient, TestDoorId);

           var openDoorsResponse = await OpenDoorsAsync(userHttpClient, TestDoorId);
        }

        private async Task<HttpResponseMessage> CreateDoorsAsync(HttpClient httpClient, long doorId)
        {
            return await httpClient.PutAsync($"v1/doors/{doorId}", null);
        }

        private async Task<HttpResponseMessage> OpenDoorsAsync(HttpClient httpClient, long doorId)
        {
            return await httpClient.PutAsync($"v1/doors/{doorId}/state/open", null);
        }
    }
}