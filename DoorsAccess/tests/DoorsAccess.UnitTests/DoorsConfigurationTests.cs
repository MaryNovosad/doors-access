using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoorsAccess.DAL;
using DoorsAccess.DAL.Repositories;
using DoorsAccess.Domain;
using DoorsAccess.Domain.DTOs;
using Moq;
using NUnit.Framework;

namespace DoorsAccess.UnitTests
{
    // namings
    public class DoorsConfigurationTests
    {
        private Mock<IDoorRepository> _doorRepositoryMock;
        private IDoorsConfigurationService _doorConfigurationService;

        const long TestDoorId = 1;

        [SetUp]
        public void Setup()
        {
            _doorRepositoryMock = new Mock<IDoorRepository>();

            _doorConfigurationService = new DoorsConfigurationService(_doorRepositoryMock.Object);
        }

        [Test]
        public async Task When_DoorDoesNotExist_Then_DoorIsCreated()
        {
            // Arrange
            var doorToCreate = GetTestDoor();
            _doorRepositoryMock.Setup(r => r.GetAsync(TestDoorId)).ReturnsAsync((Door?)null);

            // Act
            await _doorConfigurationService.CreateOrUpdateDoorAsync(doorToCreate);

            // Assert
            _doorRepositoryMock.Verify(r =>
                r.CreateAsync(It.Is<Door>(d => 
                    d.Id == doorToCreate.Id && d.Name == doorToCreate.Name && d.IsDeactivated == doorToCreate.IsDeactivated && d.State == DoorState.Closed)),
                Times.Once);

            _doorRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Door>()), Times.Never);
        }

        [Test]
        public async Task When_DoorExists_Then_DoorIsUpdated()
        {
            // Arrange
            var existingDoor = TestDoorFactory.Create();
            var doorsToUpdate = GetTestDoor(isDeactivated: true);

            _doorRepositoryMock.Setup(r => r.GetAsync(TestDoorId)).ReturnsAsync(existingDoor);

            // Act
            await _doorConfigurationService.CreateOrUpdateDoorAsync(doorsToUpdate);

            // Assert
            _doorRepositoryMock.Verify(r =>
                    r.UpdateAsync(It.Is<Door>(d =>
                        d.Id == doorsToUpdate.Id && d.Name == doorsToUpdate.Name && d.IsDeactivated == doorsToUpdate.IsDeactivated
                        && d.State == DoorState.Closed && d.UpdatedAt != existingDoor.UpdatedAt)),
                Times.Once);

            _doorRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Door>()), Times.Never);
        }

        [Test]
        public async Task When_DoorExists_But_NothingChanged_Then_DoorIsUpdated()
        {
            // Arrange
            var existingDoor = TestDoorFactory.Create();
            var doorsToUpdate = new DoorInfo
            {
                Id = existingDoor.Id,
                IsDeactivated = existingDoor.IsDeactivated,
                Name = existingDoor.Name
            };

            _doorRepositoryMock.Setup(r => r.GetAsync(TestDoorId)).ReturnsAsync(existingDoor);

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
                Id = TestDoorId,
                Name = "Test door",
                IsDeactivated = isDeactivated
            };
        }
    }
}
