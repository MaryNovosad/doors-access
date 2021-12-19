using System;
using System.Threading.Tasks;
using DoorsAccess.DAL.Repositories;
using DoorsAccess.Domain;
using DoorsAccess.Domain.DTO;
using DoorsAccess.Domain.Utils;
using DoorsAccess.Models;
using DoorsAccess.Models.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace DoorsAccess.UnitTests.Tests
{
    public class CreateOrUpdateDoorTests
    {
        private Mock<IDoorRepository> _doorRepositoryMock;
        private Mock<IClock> _clockMock;
        private IDoorsConfigurationService _doorConfigurationService;
        private DateTime _dateTimeNow;

        [SetUp]
        public void Setup()
        {
            _doorRepositoryMock = new Mock<IDoorRepository>();
            _clockMock = new Mock<IClock>();
            _clockMock.Setup(c => c.UtcNow()).Returns(() => _dateTimeNow);
            var loggerMock = new Mock<ILogger<DoorsConfigurationService>>();

            _doorConfigurationService = new DoorsConfigurationService(_doorRepositoryMock.Object, _clockMock.Object, loggerMock.Object);
        }

        [Test]
        public async Task When_DoorDoesNotExist_Then_DoorIsCreated()
        {
            // Arrange
            var doorToCreate = GetTestDoor();
            _doorRepositoryMock.Setup(r => r.GetAsync(TestConstants.DoorId)).ReturnsAsync((Door?)null);
            _dateTimeNow = TestConstants.DoorDateTime;

            // Act
            await _doorConfigurationService.CreateOrUpdateDoorAsync(doorToCreate);

            // Assert
            _doorRepositoryMock.Verify(r =>
                r.CreateAsync(It.Is<Door>(d => 
                    d.Id == doorToCreate.Id && d.Name == doorToCreate.Name && d.IsDeactivated == doorToCreate.IsDeactivated 
                    && d.State == DoorState.Closed && d.CreatedAt == _dateTimeNow && d.UpdatedAt == _dateTimeNow)),
                Times.Once);

            _doorRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Door>()), Times.Never);
        }

        [Test]
        public async Task When_DoorExists_Then_DoorIsUpdated()
        {
            // Arrange
            var existingDoor = TestDoorFactory.Create();
            _dateTimeNow = existingDoor.UpdatedAt.AddDays(1);
            var doorToUpdate = GetTestDoor(isDeactivated: true);

            _doorRepositoryMock.Setup(r => r.GetAsync(TestConstants.DoorId)).ReturnsAsync(existingDoor);

            // Act
            await _doorConfigurationService.CreateOrUpdateDoorAsync(doorToUpdate);

            // Assert
            _doorRepositoryMock.Verify(r =>
                    r.UpdateAsync(It.Is<Door>(d =>
                        d.Id == doorToUpdate.Id && d.Name == doorToUpdate.Name && d.IsDeactivated == doorToUpdate.IsDeactivated
                        && d.State == DoorState.Closed && d.UpdatedAt == _dateTimeNow && d.CreatedAt == existingDoor.CreatedAt)),
                Times.Once);

            _doorRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Door>()), Times.Never);
        }

        [Test]
        public async Task When_DoorExists_But_NothingChanged_Then_DoorIsNotUpdated()
        {
            // Arrange
            var existingDoor = TestDoorFactory.Create();
            var doorsToUpdate = new DoorInfo
            {
                Id = existingDoor.Id,
                IsDeactivated = existingDoor.IsDeactivated,
                Name = existingDoor.Name
            };

            _doorRepositoryMock.Setup(r => r.GetAsync(TestConstants.DoorId)).ReturnsAsync(existingDoor);

            // Act
            await _doorConfigurationService.CreateOrUpdateDoorAsync(doorsToUpdate);

            // Assert
            _doorRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Door>()), Times.Never);
            _doorRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Door>()), Times.Never);
        }

        protected DoorInfo GetTestDoor(bool isDeactivated = false)
        {
            return new DoorInfo
            {
                Id = TestConstants.DoorId,
                Name = TestConstants.DoorName,
                IsDeactivated = isDeactivated
            };
        }
    }
}
