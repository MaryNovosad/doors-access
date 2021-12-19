using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoorsAccess.Domain.Exceptions;
using DoorsAccess.Models;
using Moq;
using NUnit.Framework;

namespace DoorsAccess.UnitTests.Tests
{
    public class AllowDoorAccessTests : BaseDoorsAccessTests
    {
        [Test]
        public void When_DoorDoesNotExists_Then_DomainExceptionIsThrown()
        {
            // Act & Assert
            var exception = Assert.ThrowsAsync<DomainException>(() => _doorAccessService.AllowDoorAccessAsync(TestConstants.DoorId, new List<long> { TestConstants.UserId }));

            Assert.AreEqual(exception.ErrorType, DomainErrorType.NotFound);
        }

        [Test]
        public void When_DoorDoesNotExists_Then_DoorAccessesAreNotChanged()
        {
            // Act & Assert
            Assert.ThrowsAsync<DomainException>(() => _doorAccessService.AllowDoorAccessAsync(TestConstants.DoorId, new List<long> { TestConstants.UserId }));

            _doorAccessRepositoryMock.Verify(r => r.UpdateAsync(It.Is<IList<DoorAccess>>(accesses => !accesses.Any())),
                Times.Never());
            _doorAccessRepositoryMock.Verify(r => r.CreateAsync(It.Is<IList<DoorAccess>>(accesses => !accesses.Any())),
                Times.Never());
        }

        [Test] 
        public async Task When_DoorExists_Then_DoorAccessForUserIsCreated()
        {
            // Arrange
            _doorRepositoryMock.Setup(r => r.GetAsync(TestConstants.DoorId)).ReturnsAsync(TestDoorFactory.Create());
            _doorAccessRepositoryMock.Setup(r => r.GetAsync(TestConstants.DoorId)).ReturnsAsync(new List<DoorAccess>());

            // Act
            await _doorAccessService.AllowDoorAccessAsync(TestConstants.DoorId, new List<long>{ TestConstants.UserId });

            // Assert
            Func<DoorAccess, bool> doorAccessHasExpectedData = a => a.UserId == TestConstants.UserId && a.DoorId == TestConstants.DoorId && !a.IsDeactivated;

            _doorAccessRepositoryMock.Verify(r => r.CreateAsync(It.Is<IList<DoorAccess>>(
                    accesses => accesses.Count == 1 && accesses.Any(doorAccessHasExpectedData))),
                Times.Once());
        }

        [Test]
        public async Task When_DoorExists_And_DuplicatedUserIdsArePassed_Then_OnePerUserDoorAccessIsCreated()
        {
            // Arrange
            _doorRepositoryMock.Setup(r => r.GetAsync(TestConstants.DoorId)).ReturnsAsync(TestDoorFactory.Create());
            _doorAccessRepositoryMock.Setup(r => r.GetAsync(TestConstants.DoorId)).ReturnsAsync(new List<DoorAccess>());

            // Act
            await _doorAccessService.AllowDoorAccessAsync(TestConstants.DoorId, new List<long> { TestConstants.UserId, TestConstants.UserId });

            // Assert
            Func<DoorAccess, bool> doorAccessHasExpectedData = a => a.UserId == TestConstants.UserId && a.DoorId == TestConstants.DoorId && !a.IsDeactivated;

            _doorAccessRepositoryMock.Verify(r => r.CreateAsync(It.Is<IList<DoorAccess>>(
                    accesses => accesses.Count == 1 && accesses.Any(doorAccessHasExpectedData))),
                Times.Once());
        }

        [Test]
        public async Task When_DoorExists_But_DoorAccessForUserIsDeactivated_Then_DoorAccessIsReactivated()
        {
            // Arrange
            var existingAccesses = new List<DoorAccess>
            {
                new()
                {
                    UserId = TestConstants.UserId,
                    DoorId = TestConstants.DoorId,
                    IsDeactivated = true
                }
            };

            _doorRepositoryMock.Setup(r => r.GetAsync(TestConstants.DoorId)).ReturnsAsync(TestDoorFactory.Create());
            _doorAccessRepositoryMock.Setup(r => r.GetAsync(TestConstants.DoorId)).ReturnsAsync(existingAccesses);

            // Act
            await _doorAccessService.AllowDoorAccessAsync(TestConstants.DoorId, new List<long> { TestConstants.UserId });

            // Assert
            Func<DoorAccess, bool> doorAccessHasExpectedData = a => a.UserId == TestConstants.UserId && a.DoorId == TestConstants.DoorId && !a.IsDeactivated;

            _doorAccessRepositoryMock.Verify(r => r.UpdateAsync(It.Is<IList<DoorAccess>>(
                    accesses => accesses.Count == 1 && accesses.Any(doorAccessHasExpectedData))),
                Times.Once());
        }

        [Test]
        public async Task When_DoorExists_And_ActiveDoorAccessForUserExists_Then_DoorAccessIsNotChanged()
        {
            // Arrange
            var existingAccesses = new List<DoorAccess>
            {
                new()
                {
                    UserId = TestConstants.UserId,
                    DoorId = TestConstants.DoorId,
                    IsDeactivated = false
                }
            };

            _doorRepositoryMock.Setup(r => r.GetAsync(TestConstants.DoorId)).ReturnsAsync(TestDoorFactory.Create());
            _doorAccessRepositoryMock.Setup(r => r.GetAsync(TestConstants.DoorId)).ReturnsAsync(existingAccesses);

            // Act
            await _doorAccessService.AllowDoorAccessAsync(TestConstants.DoorId, new List<long> { TestConstants.UserId });

            // Assert
            _doorAccessRepositoryMock.Verify(r => r.UpdateAsync(It.Is<IList<DoorAccess>>(accesses => !accesses.Any())),
                Times.Once());
            _doorAccessRepositoryMock.Verify(r => r.CreateAsync(It.Is<IList<DoorAccess>>(accesses => !accesses.Any())),
                Times.Once());
        }

        [Test]
        public async Task When_DoorExists_And_NewAndExistingUsersWithDeactivatedAndAlreadyActivatedDoorAccessesArePassed_Then_NewOnesAreCreated_And_AlreadyActivatedNotChanged_And_DeactivatedAreReactivated()
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
                    DoorId = TestConstants.DoorId,
                    IsDeactivated = false
                },
                new()
                {
                    UserId = userWithDeactivatedDoorAccessId,
                    DoorId = TestConstants.DoorId,
                    IsDeactivated = true
                },
            };

            _doorRepositoryMock.Setup(r => r.GetAsync(TestConstants.DoorId)).ReturnsAsync(TestDoorFactory.Create());
            _doorAccessRepositoryMock.Setup(r => r.GetAsync(TestConstants.DoorId)).ReturnsAsync(existingAccesses);

            // Act
            await _doorAccessService.AllowDoorAccessAsync(TestConstants.DoorId, new List<long> { userWithActivatedDoorAccessId, userWithDeactivatedDoorAccessId, userWithoutDoorAccessId });

            // Assert
            Func<DoorAccess,long, bool> doorAccessHasExpectedData = (a, userId) => a.UserId == userId && a.DoorId == TestConstants.DoorId && !a.IsDeactivated;

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
