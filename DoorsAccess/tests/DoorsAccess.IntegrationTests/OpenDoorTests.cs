using System.Net;
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
        [Test]
        public async Task When_DoorExists_And_UserHasDoorAccess_Then_DoorAccessIsGranted_And_AccessGrantedEventIsSaved_And_OkStatusCodeIsReturned()
        {
            // Arrange
            using var userHttpClient = CreateHttpClient();
            using var adminHttpClient = CreateHttpClient(TestAdminId, TestAdminRole);

            await DoorsAccessAPIProxy.CreateDoorAsync(adminHttpClient, new CreateOrUpdateDoorRequest
            {
                DoorId = TestDoorId,
                DoorName = "Clay office entrance door",
                IsDeactivated = false
            });

            await DoorsAccessAPIProxy.AllowDoorAccessAsync(adminHttpClient, TestDoorId, new AllowDoorAccessRequest
            {
                UsersIds = new List<long> { TestUserId }
            });

            // Act
            var openDoorsResponse = await DoorsAccessAPIProxy.OpenDoorAsync(userHttpClient, TestDoorId);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, openDoorsResponse.StatusCode);

            var getDoorResponse = await DoorsAccessAPIProxy.GetDoorAsync(adminHttpClient, TestDoorId);
            Assert.AreEqual(DoorState.AccessGranted, getDoorResponse.Result.State);

            var userDoorAccessHistory = await DoorsAccessAPIProxy.GetDoorAccessHistoryAsync(userHttpClient, TestUserId);
            var eventLog = userDoorAccessHistory.Result.DoorEvents.SingleOrDefault();
            Assert.IsNotNull(eventLog);
            Assert.IsTrue(eventLog.DoorId == TestDoorId && eventLog.UserId == TestUserId && eventLog.Event == DoorEvent.AccessGranted);
        }

        [Test]
        public async Task When_DoorExists_But_UserDoesNotHaveAccess_Then_DoorAccessIsDenied_And_AccessDeniedEventIsSaved_And_NotFoundStatusCodeIsReturned()
        {
            // Arrange
            using var userHttpClient = CreateHttpClient();
            using var adminHttpClient = CreateHttpClient(TestAdminId, TestAdminRole);

            await DoorsAccessAPIProxy.CreateDoorAsync(adminHttpClient, new CreateOrUpdateDoorRequest
            {
                DoorId = TestDoorId,
                DoorName = "Clay office entrance door",
                IsDeactivated = false
            });

            // Act
            var openDoorsResponse = await DoorsAccessAPIProxy.OpenDoorAsync(userHttpClient, TestDoorId);

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, openDoorsResponse.StatusCode);

            var getDoorResponse = await DoorsAccessAPIProxy.GetDoorAsync(adminHttpClient, TestDoorId);
            Assert.AreEqual(DoorState.Closed, getDoorResponse.Result.State);

            var userDoorAccessHistory = await DoorsAccessAPIProxy.GetDoorAccessHistoryAsync(userHttpClient, TestUserId);
            var eventLog = userDoorAccessHistory.Result.DoorEvents.SingleOrDefault();
            Assert.IsNotNull(eventLog);
            Assert.IsTrue(eventLog.DoorId == TestDoorId && eventLog.UserId == TestUserId && eventLog.Event == DoorEvent.AccessDenied);
        }

        [Test]
        public async Task When_DeactivatedDoorExists_And_UserHasDoorAccess_Then_DoorAccessIsDenied_And_DeactivatedDoorAccessAttemptEventIsSaved_And_NotFoundStatusCodeIsReturned()
        {
            // Arrange
            using var userHttpClient = CreateHttpClient();
            using var adminHttpClient = CreateHttpClient(TestAdminId, TestAdminRole);

            await DoorsAccessAPIProxy.CreateDoorAsync(adminHttpClient, new CreateOrUpdateDoorRequest
            {
                DoorId = TestDoorId,
                DoorName = "Clay office entrance door",
                IsDeactivated = true
            });

            await DoorsAccessAPIProxy.AllowDoorAccessAsync(adminHttpClient, TestDoorId, new AllowDoorAccessRequest
            {
                UsersIds = new List<long> { TestUserId }
            });

            // Act
            var openDoorsResponse = await DoorsAccessAPIProxy.OpenDoorAsync(userHttpClient, TestDoorId);

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, openDoorsResponse.StatusCode);

            var getDoorResponse = await DoorsAccessAPIProxy.GetDoorAsync(adminHttpClient, TestDoorId);
            Assert.AreEqual(DoorState.Closed, getDoorResponse.Result.State);

            var userDoorAccessHistory = await DoorsAccessAPIProxy.GetDoorAccessHistoryAsync(userHttpClient, TestUserId);
            var eventLog = userDoorAccessHistory.Result.DoorEvents.SingleOrDefault();
            Assert.IsNotNull(eventLog);
            Assert.IsTrue(eventLog.DoorId == TestDoorId && eventLog.UserId == TestUserId && eventLog.Event == DoorEvent.DeactivatedDoorAccessAttempt);
        }

        [Test]
        public async Task When_DoorDoesNotExist_Then_NotFoundStatusCodeIsReturned()
        {
            // Arrange
            using var userHttpClient = CreateHttpClient();
            using var adminHttpClient = CreateHttpClient(TestAdminId, TestAdminRole);

            // Act
            var openDoorsResponse = await DoorsAccessAPIProxy.OpenDoorAsync(userHttpClient, TestDoorId);

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, openDoorsResponse.StatusCode);
        }
    }

    public class HttpResponse<T>
    {
        public T? Result { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }

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
}