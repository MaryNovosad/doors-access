using System.Net;
using DoorsAccess.API.Requests;
using DoorsAccess.IntegrationTests.SetUp;
using NUnit.Framework;

namespace DoorsAccess.IntegrationTests.Tests
{
    public class CreateOrUpdateDoorTests: IntegrationTestBase
    {
        [Test]
        public async Task When_RequiredRequestParametersAreNotPassed_Then_BadRequestIsReturned()
        {
            // Arrange
            using var adminHttpClient = CreateHttpClient(TestConstants.TestAdminId, TestConstants.TestAdminRole);
            var invalidRequest = new CreateOrUpdateDoorRequest();

            // Act
            var createDoorResponse = await DoorsAccessAPIProxy.CreateDoorAsync(adminHttpClient, invalidRequest);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, createDoorResponse.StatusCode);
        }

        [Test]
        public async Task When_TooLongDoorNameIsPassed_Then_BadRequestIsReturned()
        {
            // Arrange
            using var adminHttpClient = CreateHttpClient(TestConstants.TestAdminId, TestConstants.TestAdminRole);
            var invalidRequest = new CreateOrUpdateDoorRequest
            {
                DoorId = 1,
                DoorName = "Longer than 30 symbols door name which is not supported"
            };

            // Act
            var createDoorResponse = await DoorsAccessAPIProxy.CreateDoorAsync(adminHttpClient, invalidRequest);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, createDoorResponse.StatusCode);
        }

        [Test]
        public async Task When_ValidDoorRequestIsPassed_Then_OkStatusCodeIsReturned_And_DoorIsCreated()
        {
            // Arrange
            using var adminHttpClient = CreateHttpClient(TestConstants.TestAdminId, TestConstants.TestAdminRole);
            var validRequest = new CreateOrUpdateDoorRequest
            {
                DoorId = TestConstants.TestDoorId,
                DoorName = TestConstants.TestDoorName
            };

            // Act
            var createDoorResponse = await DoorsAccessAPIProxy.CreateDoorAsync(adminHttpClient, validRequest);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, createDoorResponse.StatusCode);

            var doorResponse = await DoorsAccessAPIProxy.GetDoorAsync(adminHttpClient, TestConstants.TestDoorId);
            Assert.IsNotNull(doorResponse.Result);
            Assert.AreEqual(doorResponse.Result.Id, TestConstants.TestDoorId);
            Assert.AreEqual(doorResponse.Result.Name, TestConstants.TestDoorName);
        }
    }
}
