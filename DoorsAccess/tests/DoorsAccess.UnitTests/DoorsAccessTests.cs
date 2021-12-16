using System;
using System.Threading.Tasks;
using DoorsAccess.DAL;
using DoorsAccess.DAL.Repositories;
using DoorsAccess.Domain;
using DoorsAccess.Domain.Exceptions;
using DoorsAccess.IoT.Integration;
using Moq;
using NUnit.Framework;

namespace DoorsAccess.UnitTests
{
    public class BaseDoorsAccessTests
    {
        protected IDoorsAccessService _doorAccessService;
        protected Mock<IDoorRepository> _doorRepositoryMock;
        protected Mock<IDoorEventLogRepository> _doorEventLogRepositoryMock;
        protected Mock<IDoorAccessRepository> _doorAccessRepositoryMock;
        protected Mock<IIoTDeviceProxy> _ioTDeviceProxyMock;

        protected const long TestUserId = 2;
        protected const long TestDoorId = 1;

        [SetUp]
        public void Setup()
        {
            _doorRepositoryMock = new Mock<IDoorRepository>();
            _doorAccessRepositoryMock = new Mock<IDoorAccessRepository>();
            _doorEventLogRepositoryMock = new Mock<IDoorEventLogRepository>();
            _ioTDeviceProxyMock = new Mock<IIoTDeviceProxy>();
            _doorAccessService = new DoorsAccessService(_doorRepositoryMock.Object, _doorAccessRepositoryMock.Object, _ioTDeviceProxyMock.Object, _doorEventLogRepositoryMock.Object);
        }
    }

    public class TestDoorFactory
    {
        // move constants
        public const long TestDoorId = 1;
        public const string TestDoorName = "Test door";

        public static Door Create(long doorId = TestDoorId, string doorName = TestDoorName, bool isDeactivated = false)
        {
            return new Door
            {
                Id = doorId,
                Name = doorName,
                IsDeactivated = isDeactivated,
                CreatedAt = new DateTime(2021, 12, 1),
                UpdatedAt = new DateTime(2021, 12, 1),
                State = DoorState.Closed
            };
        }
    }
}