using DoorsAccess.DAL;
using DoorsAccess.DAL.Repositories;
using DoorsAccess.Domain.Exceptions;
using DoorsAccess.Domain.Utils;
using DoorsAccess.IoT.Integration;
using Microsoft.Extensions.Logging;

namespace DoorsAccess.Domain
{
    public class DoorsAccessService : IDoorsAccessService
    {
        private readonly IDoorRepository _doorRepository;
        private readonly IDoorAccessRepository _doorAccessRepository;
        private readonly IIoTDeviceProxy _ioTDeviceProxy;
        private readonly IDoorEventLogRepository _doorEventLogRepository;
        private readonly IClock _clock;
        private readonly ILogger<DoorsAccessService> _logger;

        public DoorsAccessService(IDoorRepository doorRepository, IDoorAccessRepository doorAccessRepository,
            IIoTDeviceProxy ioTDoorProxy, IDoorEventLogRepository doorEventLogRepository, IClock clock, ILogger<DoorsAccessService> logger)
        {
            _doorRepository = doorRepository;
            _doorAccessRepository = doorAccessRepository;
            _ioTDeviceProxy = ioTDoorProxy;
            _doorEventLogRepository = doorEventLogRepository;
            _clock = clock;
            _logger = logger;
        }

        public async Task OpenDoorAsync(long doorId, long userId)
        {
            var door = await _doorRepository.GetAsync(doorId);

            if (door == null)
            {
                throw new DomainException(DomainErrorType.NotFound, $"Door {doorId} does not exist");
            }

            var doorEvent = await DetermineDoorEventAsync(door, userId);

            var doorEventLog = new DoorEventLog
            {
                DoorId = door.Id,
                UserId = userId,
                Event = doorEvent,
                TimeStamp = _clock.UtcNow()
            };

            await _doorEventLogRepository.AddAsync(doorEventLog);

            switch (doorEvent)
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
                    throw new InvalidOperationException("Door event is not supported");
            }

            _logger.LogInformation($"Door {doorId} access was granted to user {userId}");
        }

        public async Task AllowDoorAccessAsync(long doorId, IList<long> usersIds)
        {
            var door = await _doorRepository.GetAsync(doorId);

            if (door == null)
            {
                throw new DomainException(DomainErrorType.NotFound, $"Door {doorId} does not exist");
            }

            var existingDoorAccesses = await _doorAccessRepository.GetAsync(doorId);
            var existingUsersIds = existingDoorAccesses.Select(a => a.UserId);

            var utcNow = _clock.UtcNow();

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

            _logger.LogInformation($"Users {string.Join(", ", usersIds)} are allowed to access door {doorId}");
        }

        private async Task<DoorEvent> DetermineDoorEventAsync(Door door, long userId)
        {
            var userHasAccessToDoors = await _doorAccessRepository.CanAccessAsync(userId, door.Id);

            if (!userHasAccessToDoors)
            {
                return DoorEvent.AccessDenied;
            }

            if (door.IsDeactivated)
            {
                return DoorEvent.DeactivatedDoorAccessAttempt;
            }

            return DoorEvent.AccessGranted;
        }
    }
}