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

        public async Task OpenDoorAsync(long doorId, long userId)
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

        public async Task AllowDoorAccessAsync(long doorId, IList<long> usersIds)
        {
            // if doors are deactivated?
            var door = await _doorRepository.GetAsync(doorId);

            if (door == null)
            {
                throw new DomainException(DomainErrorType.NotFound, $"Door {doorId} does not exist");
            }

            var existingDoorAccesses = await _doorAccessRepository.GetAsync(doorId);
            var existingUsersIds = existingDoorAccesses.Select(a => a.UserId);

            var utcNow = DateTime.UtcNow;

            var newAccesses = usersIds.Distinct().Except(existingUsersIds).Select(id => new DoorAccess
            {
                UserId = id,
                CreatedAt = utcNow,
                UpdatedAt = utcNow,
                DoorId = doorId,
                IsDeactivated = false
            }).ToList();

            await _doorAccessRepository.CreateAsync(newAccesses);

            var accessesToRevoke = existingDoorAccesses.Where(a => a.IsDeactivated && usersIds.Contains(a.UserId)).Select(a =>
            {
                a.IsDeactivated = false;
                return a;
            }).ToList();

            await _doorAccessRepository.UpdateAsync(accessesToRevoke);
        }

        private async Task<DoorEventLog> LogDoorAccessAttemptAsync(Door door, long userId)
        {
            var doorEventLog = new DoorEventLog
            {
                DoorId = door.Id,
                UserId = userId,
                TimeStamp = DateTime.UtcNow
            };

            var userHasAccessToDoors = await _doorAccessRepository.CanAccessAsync(userId, door.Id);

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