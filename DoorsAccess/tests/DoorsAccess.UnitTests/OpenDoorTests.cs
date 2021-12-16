using System.Threading.Tasks;
using DoorsAccess.DAL;
using DoorsAccess.Domain.Exceptions;
using Moq;
using NUnit.Framework;

namespace DoorsAccess.UnitTests
{
    // TODO: review assertions
    public class OpenDoorTests: BaseDoorsAccessTests
    {
        [Test]
        public async Task When_DoorExists_And_UserHasDoorAccess_Then_UserCanOpenDoor()
        {
            // Arrange
            _doorRepositoryMock.Setup(r => r.GetAsync(TestDoorId)).ReturnsAsync(TestDoorFactory.Create());
            _doorAccessRepositoryMock.Setup(r => r.CanAccessAsync(TestUserId, TestDoorId)).ReturnsAsync(() => true);
            // Act
            await _doorAccessService.OpenDoorAsync(TestDoorId, TestUserId);

            // Assert
            _doorRepositoryMock.Verify(r => r.ChangeStateAsync(TestDoorId, DoorState.AccessGranted), Times.Once);
            _ioTDeviceProxyMock.Verify(r => r.OpenDoor(TestUserId, TestDoorId), Times.Once);
            _doorEventLogRepositoryMock.Verify(r => r.AddAsync(
                It.Is<DoorEventLog>(l => l.UserId == TestUserId && l.DoorId == TestDoorId && l.Event == DoorEvent.AccessGranted)));
        }

        [Test]
        public void When_DoorDoesNotExists_Then_UserCanNotOpenDoor_And_DomainExceptionIsThrown()
        {
            // Act & Assert
            var exception = Assert.ThrowsAsync<DomainException>(() => _doorAccessService.OpenDoorAsync(TestDoorId, TestUserId));
            Assert.AreEqual(exception.ErrorType, DomainErrorType.NotFound);
            _doorEventLogRepositoryMock.Verify(r => r.AddAsync(It.IsAny<DoorEventLog>()), Times.Never);
        }

        [Test]
        public void When_DoorExists_But_UserDoesNotHaveDoorAccess_Then_UserCanNotOpenDoor_And_DomainExceptionIsThrown()
        {
            // Arrange
            _doorRepositoryMock.Setup(r => r.GetAsync(TestDoorId)).ReturnsAsync(TestDoorFactory.Create());

            // Act & Assert
            var exception = Assert.ThrowsAsync<DomainException>(() => _doorAccessService.OpenDoorAsync(TestDoorId, TestUserId));
            Assert.AreEqual(exception.ErrorType, DomainErrorType.AccessDenied);
            _doorEventLogRepositoryMock.Verify(r => r.AddAsync(
                It.Is<DoorEventLog>(l => l.UserId == TestUserId && l.DoorId == TestDoorId && l.Event == DoorEvent.AccessDenied)));
        }

        [Test]
        public void When_DeactivatedDoorExists_And_UserHasDoorAccess_Then_UserCanNotOpenDoor_And_DomainExceptionIsThrown()
        {
            // Arrange
            _doorRepositoryMock.Setup(r => r.GetAsync(TestDoorId)).ReturnsAsync(TestDoorFactory.Create(isDeactivated: true));
            _doorAccessRepositoryMock.Setup(r => r.CanAccessAsync(TestUserId, TestDoorId)).ReturnsAsync(() => true);

            // Act & Assert
            var exception = Assert.ThrowsAsync<DomainException>(() => _doorAccessService.OpenDoorAsync(TestDoorId, TestUserId));
            Assert.AreEqual(exception.ErrorType, DomainErrorType.NotFound);
            _doorEventLogRepositoryMock.Verify(r => r.AddAsync(
                It.Is<DoorEventLog>(l => l.UserId == TestUserId && l.DoorId == TestDoorId && l.Event == DoorEvent.DeactivatedDoorAccessAttempt)));
        }
    }
}
