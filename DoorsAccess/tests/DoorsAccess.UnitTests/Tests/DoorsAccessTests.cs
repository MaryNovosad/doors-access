using DoorsAccess.DAL.Repositories;
using DoorsAccess.Domain;
using DoorsAccess.Domain.Utils;
using DoorsAccess.IoT.Integration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace DoorsAccess.UnitTests.Tests
{
    public class BaseDoorsAccessTests
    {
        protected IDoorsAccessService _doorAccessService;
        protected Mock<IDoorRepository> _doorRepositoryMock;
        protected Mock<IDoorEventLogRepository> _doorEventLogRepositoryMock;
        protected Mock<IDoorAccessRepository> _doorAccessRepositoryMock;
        protected Mock<IIoTDeviceProxy> _ioTDeviceProxyMock;

        [SetUp]
        public void Setup()
        {
            _doorRepositoryMock = new Mock<IDoorRepository>();
            _doorAccessRepositoryMock = new Mock<IDoorAccessRepository>();
            _doorEventLogRepositoryMock = new Mock<IDoorEventLogRepository>();
            _ioTDeviceProxyMock = new Mock<IIoTDeviceProxy>();

            var clockMock = new Mock<IClock>();
            clockMock.Setup(c => c.UtcNow()).Returns(TestConstants.DoorDateTime);

            var loggerMock = new Mock<ILogger<DoorsAccessService>>();

            _doorAccessService = new DoorsAccessService(_doorRepositoryMock.Object, _doorAccessRepositoryMock.Object, _ioTDeviceProxyMock.Object, 
                _doorEventLogRepositoryMock.Object, clockMock.Object, loggerMock.Object);
        }
    }
}