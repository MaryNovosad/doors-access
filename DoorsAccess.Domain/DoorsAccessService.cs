using DoorsAccess.DAL;
using DoorsAccess.DAL.Repositories;
using DoorsAccess.Domain.Exceptions;
using DoorsAccess.IoT.Integration;

namespace DoorsAccess.Domain
{
    public class DoorsAccessService : IDoorsAccessService
    {
        private readonly IDoorRepository _doorRepository;
        private readonly IDoorAccessRepository _doorAccessRepository;
        private readonly IIoTDeviceProxy _ioTDeviceProxy;
        private readonly IDoorEventLogRepository _doorEventLogRepository;

        public DoorsAccessService(IDoorRepository doorRepository, IDoorAccessRepository doorAccessRepository,
            IIoTDeviceProxy ioTDoorProxy, IDoorEventLogRepository doorEventLogRepository)
        {
            _doorRepository = doorRepository;
            _doorAccessRepository = doorAccessRepository;
            _ioTDeviceProxy = ioTDoorProxy;
            _doorEventLogRepository = doorEventLogRepository;
        }

        public async Task GrantDoorAccessAsync(long doorId, long userId)
        {
            var door = await _doorRepository.GetAsync(doorId);

            if (door == null)
            {
                throw new DomainException(DomainErrorType.NotFound, $"Door {doorId} does not exist");
            }

            var doorEventLog = await LogDoorAccessAttemptAsync(door, userId);

            switch (doorEventLog.Event)
            {
                case DoorEvent.AccessGranted:
                    await _doorRepository.ChangeStateAsync(door.Id, DoorState.AccessGranted);
                    _ioTDeviceProxy.OpenDoor(userId, doorId);
                    break;
                case DoorEvent.AccessDenied:
                    throw new DomainException(DomainErrorType.AccessDenied, $"User {userId} doesn't have access to door {doorId}");
                case DoorEvent.DeactivatedDoorAccessAttempt:
                    throw new DomainException(DomainErrorType.NotFound, $"User {userId} tries to access deactivated door {doorId}");
                default:
                    throw new InvalidOperationException();
            }
        }

        private async Task<DoorEventLog> LogDoorAccessAttemptAsync(Door door, long userId)
        {
            var doorEventLog = new DoorEventLog
            {
                DoorId = door.Id,
                UserId = userId,
                TimeStamp = DateTime.UtcNow
            };

            var userHasAccessToDoors = await _doorAccessRepository.CanAccess(userId, door.Id, DateTime.UtcNow);

            if (!userHasAccessToDoors)
            {
                doorEventLog.Event = DoorEvent.AccessDenied;
            }
            else if (door.IsDeactivated)
            {
                doorEventLog.Event = DoorEvent.DeactivatedDoorAccessAttempt;
            }
            else
            {
                doorEventLog.Event = DoorEvent.AccessGranted;
            }

            await _doorEventLogRepository.AddAsync(doorEventLog);

            return doorEventLog;
        }
    }
}