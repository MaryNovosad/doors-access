using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoorsAccess.DAL;
using DoorsAccess.Domain.Exceptions;
using Moq;
using NUnit.Framework;

namespace DoorsAccess.UnitTests
{
    // namings
    public class AllowDoorAccessTests : BaseDoorsAccessTests
    {
        [Test]
        public void When_DoorDoesNotExists_Then_DomainExceptionIsThrown()
        {
            // Act & Assert
            var exception = Assert.ThrowsAsync<DomainException>(() => _doorAccessService.AllowDoorAccessAsync(TestDoorId, new List<long> { TestUserId }));

            Assert.AreEqual(exception.ErrorType, DomainErrorType.NotFound);
            _doorAccessRepositoryMock.Verify(r => r.UpdateAsync(It.Is<IList<DoorAccess>>(accesses => !accesses.Any())),
                Times.Never());
            _doorAccessRepositoryMock.Verify(r => r.CreateAsync(It.Is<IList<DoorAccess>>(accesses => !accesses.Any())),
                Times.Never());
        }

        [Test] 
        public async Task When_DoorExists_Then_UserIsAllowedToAccessDoor()
        {
            // Arrange
            _doorRepositoryMock.Setup(r => r.GetAsync(TestDoorId)).ReturnsAsync(TestDoorFactory.Create());
            _doorAccessRepositoryMock.Setup(r => r.GetAsync(TestDoorId)).ReturnsAsync(new List<DoorAccess>());

            // Act
            await _doorAccessService.AllowDoorAccessAsync(TestDoorId, new List<long>{ TestUserId });

            // Assert
            Func<DoorAccess, bool> doorAccessHasExpectedData = a => a.UserId == TestUserId && a.DoorId == TestDoorId && !a.IsDeactivated;

            _doorAccessRepositoryMock.Verify(r => r.CreateAsync(It.Is<IList<DoorAccess>>(
                    accesses => accesses.Count == 1 && accesses.Any(doorAccessHasExpectedData))),
                Times.Once());
        }

        [Test]
        public async Task DuplicatedUsers()
        {
            // Arrange
            _doorRepositoryMock.Setup(r => r.GetAsync(TestDoorId)).ReturnsAsync(TestDoorFactory.Create());
            _doorAccessRepositoryMock.Setup(r => r.GetAsync(TestDoorId)).ReturnsAsync(new List<DoorAccess>());

            // Act
            await _doorAccessService.AllowDoorAccessAsync(TestDoorId, new List<long> { TestUserId, TestUserId });

            // Assert
            Func<DoorAccess, bool> doorAccessHasExpectedData = a => a.UserId == TestUserId && a.DoorId == TestDoorId && !a.IsDeactivated;

            _doorAccessRepositoryMock.Verify(r => r.CreateAsync(It.Is<IList<DoorAccess>>(
                    accesses => accesses.Count == 1 && accesses.Any(doorAccessHasExpectedData))),
                Times.Once());
        }

        [Test]
        public async Task RevokeAccess()
        {
            // Arrange
            var existingAccesses = new List<DoorAccess>
            {
                new()
                {
                    UserId = TestUserId,
                    DoorId = TestDoorId,
                    IsDeactivated = true
                }
            };

            _doorRepositoryMock.Setup(r => r.GetAsync(TestDoorId)).ReturnsAsync(TestDoorFactory.Create());
            _doorAccessRepositoryMock.Setup(r => r.GetAsync(TestDoorId)).ReturnsAsync(existingAccesses);

            // Act
            await _doorAccessService.AllowDoorAccessAsync(TestDoorId, new List<long> { TestUserId });

            // Assert
            Func<DoorAccess, bool> doorAccessHasExpectedData = a => a.UserId == TestUserId && a.DoorId == TestDoorId && !a.IsDeactivated;

            _doorAccessRepositoryMock.Verify(r => r.UpdateAsync(It.Is<IList<DoorAccess>>(
                    accesses => accesses.Count == 1 && accesses.Any(doorAccessHasExpectedData))),
                Times.Once());
        }


        [Test]
        public async Task UserExists_DoNothing()
        {
            // Arrange
            var existingAccesses = new List<DoorAccess>
            {
                new()
                {
                    UserId = TestUserId,
                    DoorId = TestDoorId,
                    IsDeactivated = false
                }
            };

            _doorRepositoryMock.Setup(r => r.GetAsync(TestDoorId)).ReturnsAsync(TestDoorFactory.Create());
            _doorAccessRepositoryMock.Setup(r => r.GetAsync(TestDoorId)).ReturnsAsync(existingAccesses);

            // Act
            await _doorAccessService.AllowDoorAccessAsync(TestDoorId, new List<long> { TestUserId });

            // Assert
            _doorAccessRepositoryMock.Verify(r => r.UpdateAsync(It.Is<IList<DoorAccess>>(accesses => !accesses.Any())),
                Times.Once());
            _doorAccessRepositoryMock.Verify(r => r.CreateAsync(It.Is<IList<DoorAccess>>(accesses => !accesses.Any())),
                Times.Once());
        }

        [Test]
        public async Task DifferentUsers()
        {
            // Arrange
            var userWithActivatedDoorAccessId = 1;
            var userWithDeactivatedDoorAccessId = 2;
            var userWithoutDoorAccessId = 3;

            var existingAccesses = new List<DoorAccess>
            {
                new()
                {
                    UserId = userWithActivatedDoorAccessId,
                    DoorId = TestDoorId,
                    IsDeactivated = false
                },
                new()
                {
                    UserId = userWithDeactivatedDoorAccessId,
                    DoorId = TestDoorId,
                    IsDeactivated = true
                },
            };

            _doorRepositoryMock.Setup(r => r.GetAsync(TestDoorId)).ReturnsAsync(TestDoorFactory.Create());
            _doorAccessRepositoryMock.Setup(r => r.GetAsync(TestDoorId)).ReturnsAsync(existingAccesses);

            // Act
            await _doorAccessService.AllowDoorAccessAsync(TestDoorId, new List<long> { userWithActivatedDoorAccessId, userWithDeactivatedDoorAccessId, userWithoutDoorAccessId });

            // Assert
            Func<DoorAccess,long, bool> doorAccessHasExpectedData = (a, userId) => a.UserId == userId && a.DoorId == TestDoorId && !a.IsDeactivated;

            _doorAccessRepositoryMock.Verify(r => r.CreateAsync(It.Is<IList<DoorAccess>>(
                    accesses => 
                        accesses.Count == 1 && accesses.Any(a => doorAccessHasExpectedData(a, userWithoutDoorAccessId)))),
                Times.Once());

            _doorAccessRepositoryMock.Verify(r => r.UpdateAsync(It.Is<IList<DoorAccess>>(
                    accesses => 
                        accesses.Count == 1 && accesses.Any(a => doorAccessHasExpectedData(a, userWithDeactivatedDoorAccessId)))),
                Times.Once());
        }
    }
}
