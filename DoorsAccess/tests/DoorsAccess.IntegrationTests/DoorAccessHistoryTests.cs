using System.Net;
using DoorsAccess.API.Requests;
using DoorsAccess.API.Responses;
using DoorsAccess.IntegrationTests.SetUp;
using NUnit.Framework;

namespace DoorsAccess.IntegrationTests
{
    public class DoorAccessHistoryTests: IntegrationTestBase
    {
        [Test]
        public async Task When_NoDoorAccessEventsExist_Then_OkStatusCodeWithEmptyCollectionIsReturned()
        {
            // Arrange
            using var adminHttpClient = CreateHttpClient(TestAdminId, TestAdminRole);

            // Act
            var userDoorAccessHistory = await DoorsAccessAPIProxy.GetDoorAccessHistoryAsync(adminHttpClient);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, userDoorAccessHistory.StatusCode);
            Assert.IsEmpty(userDoorAccessHistory.Result.DoorEvents);
        }

        [Test]
        public async Task When_UserRequestsHistoryOfOtherUser_Then_ForbiddenStatusCodeIsReturned()
        {
            // Arrange
            using var httpClient = CreateHttpClient();
            var otherUserId = TestUserId + 1;

            // Act
            var userDoorAccessHistory = await DoorsAccessAPIProxy.GetDoorAccessHistoryAsync(httpClient, otherUserId);

            // Assert
            Assert.AreEqual(HttpStatusCode.Forbidden, userDoorAccessHistory.StatusCode);
        }

        [Test]
        public async Task When_AdminRequestsHistoryOfOtherUser_Then_OkStatusCodeWithUserHistoryIsReturned()
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
            await DoorsAccessAPIProxy.OpenDoorAsync(userHttpClient, TestDoorId);

            // Act
            var userDoorAccessHistory = await DoorsAccessAPIProxy.GetDoorAccessHistoryAsync(adminHttpClient, TestUserId);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, userDoorAccessHistory.StatusCode);
            var eventLog = userDoorAccessHistory.Result.DoorEvents.SingleOrDefault();
            Assert.IsNotNull(eventLog);
            Assert.IsTrue(eventLog.DoorId == TestDoorId && eventLog.UserId == TestUserId && eventLog.Event == DoorEvent.AccessGranted);
        }

        [Test]
        public async Task When_UserIsNotAdmin_Then_ForbiddenStatusCodeIsReturned()
        {
            // Arrange
            using var httpClient = CreateHttpClient();

            // Act
            var userDoorAccessHistory = await DoorsAccessAPIProxy.GetDoorAccessHistoryAsync(httpClient);

            // Assert
            Assert.AreEqual(HttpStatusCode.Forbidden, userDoorAccessHistory.StatusCode);
        }
    }
}
