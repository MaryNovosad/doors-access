using DoorsAccess.DAL.Repositories;
using DoorsAccess.Domain;
using DoorsAccess.Domain.Utils;
using DoorsAccess.Messaging;
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
        protected Mock<IDoorAccessMessageSender> _messageSenderMock;

        [SetUp]
        public void Setup()
        {
            _doorRepositoryMock = new Mock<IDoorRepository>();
            _doorAccessRepositoryMock = new Mock<IDoorAccessRepository>();
            _doorEventLogRepositoryMock = new Mock<IDoorEventLogRepository>();
            _messageSenderMock = new Mock<IDoorAccessMessageSender>();

            var clockMock = new Mock<IClock>();
            clockMock.Setup(c => c.UtcNow()).Returns(TestConstants.DoorDateTime);

            var loggerMock = new Mock<ILogger<DoorsAccessService>>();

            _doorAccessService = new DoorsAccessService(_doorRepositoryMock.Object, _doorAccessRepositoryMock.Object, _doorEventLogRepositoryMock.Object,
                _messageSenderMock.Object, clockMock.Object, loggerMock.Object);
        }
    }
}