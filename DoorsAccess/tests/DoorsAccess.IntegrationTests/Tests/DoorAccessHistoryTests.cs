using System.Net;
using DoorsAccess.API.Requests;
using DoorsAccess.API.Responses;
using DoorsAccess.IntegrationTests.SetUp;
using NUnit.Framework;

namespace DoorsAccess.IntegrationTests.Tests
{
    public class DoorAccessHistoryTests: IntegrationTestBase
    {
        [Test]
        public async Task When_NoDoorAccessEventsExist_Then_OkStatusCodeWithEmptyCollectionIsReturned()
        {
            // Arrange
            using var adminHttpClient = CreateHttpClient(TestConstants.TestAdminId, TestConstants.TestAdminRole);

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
            var otherUserId = TestConstants.TestUserId + 1;

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
            using var adminHttpClient = CreateHttpClient(TestConstants.TestAdminId, TestConstants.TestAdminRole);

            await DoorsAccessAPIProxy.CreateDoorAsync(adminHttpClient, new CreateOrUpdateDoorRequest
            {
                DoorId = TestConstants.TestDoorId,
                DoorName = TestConstants.TestDoorName,
                IsDeactivated = false
            });
            await DoorsAccessAPIProxy.AllowDoorAccessAsync(adminHttpClient, TestConstants.TestDoorId, new AllowDoorAccessRequest
            {
                UsersIds = new List<long> { TestConstants.TestUserId }
            });
            await DoorsAccessAPIProxy.OpenDoorAsync(userHttpClient, TestConstants.TestDoorId);

            // Act
            var userDoorAccessHistory = await DoorsAccessAPIProxy.GetDoorAccessHistoryAsync(adminHttpClient, TestConstants.TestUserId);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, userDoorAccessHistory.StatusCode);
            var eventLog = userDoorAccessHistory.Result.DoorEvents.SingleOrDefault();
            Assert.IsNotNull(eventLog);
            Assert.IsTrue(eventLog.DoorId == TestConstants.TestDoorId && eventLog.UserId == TestConstants.TestUserId && eventLog.Event == DoorEvent.AccessGranted);
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
