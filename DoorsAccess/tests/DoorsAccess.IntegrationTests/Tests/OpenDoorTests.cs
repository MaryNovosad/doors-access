using System.Net;
using DoorsAccess.API.Requests;
using DoorsAccess.API.Responses;
using DoorsAccess.IntegrationTests.SetUp;
using NUnit.Framework;

namespace DoorsAccess.IntegrationTests.Tests
{
    public class OpenDoorTests : IntegrationTestBase
    {
        [Test]
        public async Task When_DoorExists_And_UserHasDoorAccess_Then_DoorAccessIsGranted_And_AccessGrantedEventIsSaved_And_OkStatusCodeIsReturned()
        {
            // Arrange
            using var userHttpClient = CreateHttpClient();
            using var adminHttpClient = CreateHttpClient(TestConstants.TestAdminId, TestConstants.TestAdminRole);

            await DoorsAccessAPIProxy.CreateDoorAsync(adminHttpClient, CreateTestDoorRequest);

            await DoorsAccessAPIProxy.AllowDoorAccessAsync(adminHttpClient, TestConstants.TestDoorId, new AllowDoorAccessRequest
            {
                UsersIds = new List<long> { TestConstants.TestUserId }
            });

            // Act
            var openDoorsResponse = await DoorsAccessAPIProxy.OpenDoorAsync(userHttpClient, TestConstants.TestDoorId);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, openDoorsResponse.StatusCode);

            var getDoorResponse = await DoorsAccessAPIProxy.GetDoorAsync(adminHttpClient, TestConstants.TestDoorId);
            Assert.AreEqual(DoorState.AccessGranted, getDoorResponse.Result.State);

            var userDoorAccessHistory = await DoorsAccessAPIProxy.GetDoorAccessHistoryAsync(userHttpClient, TestConstants.TestUserId);
            var eventLog = userDoorAccessHistory.Result.DoorEvents.SingleOrDefault();
            Assert.IsNotNull(eventLog);
            Assert.IsTrue(eventLog.DoorId == TestConstants.TestDoorId && eventLog.UserId == TestConstants.TestUserId && eventLog.Event == DoorEvent.AccessGranted);
        }

        [Test]
        public async Task When_DoorExists_But_UserDoesNotHaveAccess_Then_DoorAccessIsDenied_And_AccessDeniedEventIsSaved_And_NotFoundStatusCodeIsReturned()
        {
            // Arrange
            using var userHttpClient = CreateHttpClient();
            using var adminHttpClient = CreateHttpClient(TestConstants.TestAdminId, TestConstants.TestAdminRole);

            await DoorsAccessAPIProxy.CreateDoorAsync(adminHttpClient, CreateTestDoorRequest);

            // Act
            var openDoorsResponse = await DoorsAccessAPIProxy.OpenDoorAsync(userHttpClient, TestConstants.TestDoorId);

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, openDoorsResponse.StatusCode);

            var getDoorResponse = await DoorsAccessAPIProxy.GetDoorAsync(adminHttpClient, TestConstants.TestDoorId);
            Assert.AreEqual(DoorState.Closed, getDoorResponse.Result.State);

            var userDoorAccessHistory = await DoorsAccessAPIProxy.GetDoorAccessHistoryAsync(userHttpClient, TestConstants.TestUserId);
            var eventLog = userDoorAccessHistory.Result.DoorEvents.SingleOrDefault();
            Assert.IsNotNull(eventLog);
            Assert.IsTrue(eventLog.DoorId == TestConstants.TestDoorId && eventLog.UserId == TestConstants.TestUserId && eventLog.Event == DoorEvent.AccessDenied);
        }

        [Test]
        public async Task When_DeactivatedDoorExists_And_UserHasDoorAccess_Then_DoorAccessIsDenied_And_DeactivatedDoorAccessAttemptEventIsSaved_And_NotFoundStatusCodeIsReturned()
        {
            // Arrange
            using var userHttpClient = CreateHttpClient();
            using var adminHttpClient = CreateHttpClient(TestConstants.TestAdminId, TestConstants.TestAdminRole);
            var deactivatedDoorRequest = CreateTestDoorRequest;
            deactivatedDoorRequest.IsDeactivated = true;

            await DoorsAccessAPIProxy.CreateDoorAsync(adminHttpClient, deactivatedDoorRequest);

            await DoorsAccessAPIProxy.AllowDoorAccessAsync(adminHttpClient, TestConstants.TestDoorId, new AllowDoorAccessRequest
            {
                UsersIds = new List<long> { TestConstants.TestUserId }
            });

            // Act
            var openDoorsResponse = await DoorsAccessAPIProxy.OpenDoorAsync(userHttpClient, TestConstants.TestDoorId);

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, openDoorsResponse.StatusCode);

            var getDoorResponse = await DoorsAccessAPIProxy.GetDoorAsync(adminHttpClient, TestConstants.TestDoorId);
            Assert.AreEqual(DoorState.Closed, getDoorResponse.Result.State);

            var userDoorAccessHistory = await DoorsAccessAPIProxy.GetDoorAccessHistoryAsync(userHttpClient, TestConstants.TestUserId);
            var eventLog = userDoorAccessHistory.Result.DoorEvents.SingleOrDefault();
            Assert.IsNotNull(eventLog);
            Assert.IsTrue(eventLog.DoorId == TestConstants.TestDoorId && eventLog.UserId == TestConstants.TestUserId && eventLog.Event == DoorEvent.DeactivatedDoorAccessAttempt);
        }

        [Test]
        public async Task When_DoorDoesNotExist_Then_NotFoundStatusCodeIsReturned()
        {
            // Arrange
            using var userHttpClient = CreateHttpClient();
            using var adminHttpClient = CreateHttpClient(TestConstants.TestAdminId, TestConstants.TestAdminRole);

            // Act
            var openDoorsResponse = await DoorsAccessAPIProxy.OpenDoorAsync(userHttpClient, TestConstants.TestDoorId);

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, openDoorsResponse.StatusCode);
        }

        private CreateOrUpdateDoorRequest CreateTestDoorRequest => new()
        {
            DoorId = TestConstants.TestDoorId,
            DoorName = TestConstants.TestDoorName,
            IsDeactivated = false
        };
    }
}