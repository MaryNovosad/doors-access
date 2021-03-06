using System.Threading.Tasks;
using DoorsAccess.Domain.Exceptions;
using DoorsAccess.Messaging.Messages;
using DoorsAccess.Models;
using DoorsAccess.Models.Enums;
using Moq;
using NUnit.Framework;

namespace DoorsAccess.UnitTests.Tests
{
    public class OpenDoorTests: BaseDoorsAccessTests
    {
        [Test]
        public async Task When_DoorExists_And_UserHasDoorAccess_Then_DoorStateIsChangedToAccessGranted()
        {
            // Arrange
            _doorRepositoryMock.Setup(r => r.GetAsync(TestConstants.DoorId)).ReturnsAsync(TestDoorFactory.Create());
            _doorAccessRepositoryMock.Setup(r => r.CanAccessAsync(TestConstants.UserId, TestConstants.DoorId)).ReturnsAsync(() => true);
            
            // Act
            await _doorAccessService.OpenDoorAsync(TestConstants.DoorId, TestConstants.UserId);

            // Assert
            _doorRepositoryMock.Verify(r => r.ChangeStateAsync(TestConstants.DoorId, DoorState.AccessGranted), Times.Once);
        }

        [Test]
        public async Task When_DoorExists_And_UserHasDoorAccess_Then_OpenDoorAccessCommandMessageIsSent()
        {
            // Arrange
            _doorRepositoryMock.Setup(r => r.GetAsync(TestConstants.DoorId)).ReturnsAsync(TestDoorFactory.Create());
            _doorAccessRepositoryMock.Setup(r => r.CanAccessAsync(TestConstants.UserId, TestConstants.DoorId)).ReturnsAsync(() => true);
            
            // Act
            await _doorAccessService.OpenDoorAsync(TestConstants.DoorId, TestConstants.UserId);

            // Assert
            _messageSenderMock.Verify(s => s.SendAsync(
                It.Is<DoorAccessCommand>(m => m.UserId == TestConstants.UserId && m.DoorId == TestConstants.DoorId && m.Type == DoorCommandType.Open)), Times.Once);
        }

        [Test]
        public async Task When_DoorExists_And_UserHasDoorAccess_Then_DoorAccessGrantedEventLogIsCreated()
        {
            // Arrange
            _doorRepositoryMock.Setup(r => r.GetAsync(TestConstants.DoorId)).ReturnsAsync(TestDoorFactory.Create());
            _doorAccessRepositoryMock.Setup(r => r.CanAccessAsync(TestConstants.UserId, TestConstants.DoorId)).ReturnsAsync(() => true);
            
            // Act
            await _doorAccessService.OpenDoorAsync(TestConstants.DoorId, TestConstants.UserId);

            // Assert
            _doorEventLogRepositoryMock.Verify(r => r.AddAsync(
                It.Is<DoorEventLog>(l => l.UserId == TestConstants.UserId && l.DoorId == TestConstants.DoorId && l.EventType == DoorEventType.AccessGranted)));
        }

        [Test]
        public void When_DoorDoesNotExists_Then_DomainExceptionIsThrown()
        {
            // Act & Assert
            var exception = Assert.ThrowsAsync<DomainException>(() => _doorAccessService.OpenDoorAsync(TestConstants.DoorId, TestConstants.UserId));
            Assert.AreEqual(exception.ErrorType, DomainErrorType.NotFound);
        }

        [Test]
        public void When_DoorDoesNotExists_Then_DoorAccessEventLogIsNotCreated()
        {
            // Act & Assert
            Assert.ThrowsAsync<DomainException>(() => _doorAccessService.OpenDoorAsync(TestConstants.DoorId, TestConstants.UserId));
            _doorEventLogRepositoryMock.Verify(r => r.AddAsync(It.IsAny<DoorEventLog>()), Times.Never);
        }

        [Test]
        public void When_DoorDoesNotExists_Then_DoorAccessCommandMessageIsNotSent()
        {
            // Act & Assert
            Assert.ThrowsAsync<DomainException>(() => _doorAccessService.OpenDoorAsync(TestConstants.DoorId, TestConstants.UserId));
            _messageSenderMock.Verify(s => s.SendAsync(It.IsAny<DoorAccessCommand>()), Times.Never);
        }

        [Test]
        public void When_DoorDoesNotExists_Then_DoorStateIsNotChanged()
        {
            // Act & Assert
            Assert.ThrowsAsync<DomainException>(() => _doorAccessService.OpenDoorAsync(TestConstants.DoorId, TestConstants.UserId));
            _doorRepositoryMock.Verify(r => r.ChangeStateAsync(TestConstants.DoorId, It.IsAny<DoorState>()), Times.Never);
        }

        [Test]
        public void When_DoorExists_But_UserDoesNotHaveDoorAccess_Then_DomainExceptionIsThrown()
        {
            // Arrange
            _doorRepositoryMock.Setup(r => r.GetAsync(TestConstants.DoorId)).ReturnsAsync(TestDoorFactory.Create());

            // Act & Assert
            var exception = Assert.ThrowsAsync<DomainException>(() => _doorAccessService.OpenDoorAsync(TestConstants.DoorId, TestConstants.UserId));
            Assert.AreEqual(exception.ErrorType, DomainErrorType.AccessDenied);
        }

        [Test]
        public void When_DoorExists_But_UserDoesNotHaveDoorAccess_Then_DoorAccessDeniedEventLogIsCreated()
        {
            // Arrange
            _doorRepositoryMock.Setup(r => r.GetAsync(TestConstants.DoorId)).ReturnsAsync(TestDoorFactory.Create());

            // Act & Assert
            Assert.ThrowsAsync<DomainException>(() => _doorAccessService.OpenDoorAsync(TestConstants.DoorId, TestConstants.UserId));
            _doorEventLogRepositoryMock.Verify(r => r.AddAsync(
                It.Is<DoorEventLog>(l => l.UserId == TestConstants.UserId && l.DoorId == TestConstants.DoorId && l.EventType == DoorEventType.AccessDenied)));
        }

        [Test]
        public void When_DoorExists_But_UserDoesNotHaveDoorAccess_Then_DoorStateIsNotChanged()
        {
            // Arrange
            _doorRepositoryMock.Setup(r => r.GetAsync(TestConstants.DoorId)).ReturnsAsync(TestDoorFactory.Create());

            // Act & Assert
            Assert.ThrowsAsync<DomainException>(() => _doorAccessService.OpenDoorAsync(TestConstants.DoorId, TestConstants.UserId));
            _doorRepositoryMock.Verify(r => r.ChangeStateAsync(TestConstants.DoorId, It.IsAny<DoorState>()), Times.Never);
        }

        [Test]
        public void When_DoorExists_But_UserDoesNotHaveDoorAccess_Then_DoorAccessCommandMessageIsNotSent()
        {
            // Arrange
            _doorRepositoryMock.Setup(r => r.GetAsync(TestConstants.DoorId)).ReturnsAsync(TestDoorFactory.Create());

            // Act & Assert
            Assert.ThrowsAsync<DomainException>(() => _doorAccessService.OpenDoorAsync(TestConstants.DoorId, TestConstants.UserId));
            _messageSenderMock.Verify(s => s.SendAsync(It.IsAny<DoorAccessCommand>()), Times.Never);
        }

        [Test]
        public void When_DeactivatedDoorExists_And_UserHasDoorAccess_Then_DomainExceptionIsThrown()
        {
            // Arrange
            _doorRepositoryMock.Setup(r => r.GetAsync(TestConstants.DoorId)).ReturnsAsync(TestDoorFactory.Create(isDeactivated: true));
            _doorAccessRepositoryMock.Setup(r => r.CanAccessAsync(TestConstants.UserId, TestConstants.DoorId)).ReturnsAsync(() => true);

            // Act & Assert
            var exception = Assert.ThrowsAsync<DomainException>(() => _doorAccessService.OpenDoorAsync(TestConstants.DoorId, TestConstants.UserId));
            Assert.AreEqual(exception.ErrorType, DomainErrorType.NotFound);
        }

        [Test]
        public void When_DeactivatedDoorExists_And_UserHasDoorAccess_Then_DeactivatedDoorAccessAttemptEventLogIsCreated()
        {
            // Arrange
            _doorRepositoryMock.Setup(r => r.GetAsync(TestConstants.DoorId)).ReturnsAsync(TestDoorFactory.Create(isDeactivated: true));
            _doorAccessRepositoryMock.Setup(r => r.CanAccessAsync(TestConstants.UserId, TestConstants.DoorId)).ReturnsAsync(() => true);

            // Act & Assert
            Assert.ThrowsAsync<DomainException>(() => _doorAccessService.OpenDoorAsync(TestConstants.DoorId, TestConstants.UserId));
            _doorEventLogRepositoryMock.Verify(r => r.AddAsync(
                It.Is<DoorEventLog>(l => l.UserId == TestConstants.UserId && l.DoorId == TestConstants.DoorId && l.EventType == DoorEventType.DeactivatedDoorAccessAttempt)));
        }

        [Test]
        public void When_DeactivatedDoorExists_And_UserHasDoorAccess_Then_DoorAccessCommandMessageIsNotSent()
        {
            // Arrange
            _doorRepositoryMock.Setup(r => r.GetAsync(TestConstants.DoorId)).ReturnsAsync(TestDoorFactory.Create(isDeactivated: true));
            _doorAccessRepositoryMock.Setup(r => r.CanAccessAsync(TestConstants.UserId, TestConstants.DoorId)).ReturnsAsync(() => true);

            // Act & Assert
            Assert.ThrowsAsync<DomainException>(() => _doorAccessService.OpenDoorAsync(TestConstants.DoorId, TestConstants.UserId));
            _messageSenderMock.Verify(s => s.SendAsync(It.IsAny<DoorAccessCommand>()), Times.Never);
        }

        [Test]
        public void When_DeactivatedDoorExists_And_UserHasDoorAccess_Then_DoorStateIsNotChanged()
        {
            // Arrange
            _doorRepositoryMock.Setup(r => r.GetAsync(TestConstants.DoorId)).ReturnsAsync(TestDoorFactory.Create(isDeactivated: true));
            _doorAccessRepositoryMock.Setup(r => r.CanAccessAsync(TestConstants.UserId, TestConstants.DoorId)).ReturnsAsync(() => true);

            // Act & Assert
            Assert.ThrowsAsync<DomainException>(() => _doorAccessService.OpenDoorAsync(TestConstants.DoorId, TestConstants.UserId));
            _doorRepositoryMock.Verify(r => r.ChangeStateAsync(TestConstants.DoorId, It.IsAny<DoorState>()), Times.Never);
        }
    }
}
