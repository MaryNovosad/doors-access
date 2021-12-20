using DoorsAccess.DAL;
using DoorsAccess.Domain.Exceptions;
using DoorsAccess.Domain.Utils;
using DoorsAccess.Messaging;
using DoorsAccess.Messaging.Messages;
using DoorsAccess.Models;
using DoorsAccess.Models.Enums;
using Microsoft.Extensions.Logging;

namespace DoorsAccess.Domain
{
    public class DoorsAccessService : IDoorsAccessService
    {
        private readonly IDoorRepository _doorRepository;
        private readonly IDoorAccessRepository _doorAccessRepository;
        private readonly IDoorAccessMessageSender _doorAccessMessageSender;
        private readonly IDoorEventLogRepository _doorEventLogRepository;
        private readonly IClock _clock;
        private readonly ILogger<DoorsAccessService> _logger;

        public DoorsAccessService(IDoorRepository doorRepository, IDoorAccessRepository doorAccessRepository,
            IDoorEventLogRepository doorEventLogRepository, IDoorAccessMessageSender messageSender, IClock clock, ILogger<DoorsAccessService> logger)
        {
            _doorRepository = doorRepository ?? throw new ArgumentNullException(nameof(doorRepository));
            _doorAccessRepository = doorAccessRepository ?? throw new ArgumentNullException(nameof(doorAccessRepository));
            _doorEventLogRepository = doorEventLogRepository ?? throw new ArgumentNullException(nameof(doorEventLogRepository));
            _doorAccessMessageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task OpenDoorAsync(long doorId, long userId)
        {
            var door = await _doorRepository.GetAsync(doorId);

            if (door == null)
            {
                throw new DomainException(DomainErrorType.NotFound, $"Door {doorId} does not exist");
            }

            var now = _clock.UtcNow();
            var doorEvent = await DetermineDoorEventAsync(door, userId);

            var doorEventLog = new DoorEventLog
            {
                DoorId = door.Id,
                UserId = userId,
                EventType = doorEvent,
                TimeStamp = now
            };

            await _doorEventLogRepository.AddAsync(doorEventLog);

            // TODO: Uncomment when doors access historical events functionality is extracted to a separate micro-service
            //var message = new DoorAccessMessage(door.Id, userId, now, doorEvent);
            //await _doorAccessMessageSender.SendAsync(message);

            switch (doorEvent)
            {
                case DoorEventType.AccessGranted:
                {
                    await _doorRepository.ChangeStateAsync(door.Id, DoorState.AccessGranted);

                    var command = new DoorAccessCommand(door.Id, userId, now, DoorCommandType.Open);
                    await _doorAccessMessageSender.SendAsync(command);

                    break;
                }

                case DoorEventType.AccessDenied:
                {
                    throw new DomainException(DomainErrorType.AccessDenied, $"User {userId} doesn't have access to door {doorId}");
                }

                case DoorEventType.DeactivatedDoorAccessAttempt:
                {
                    throw new DomainException(DomainErrorType.NotFound, $"User {userId} tries to access deactivated door {doorId}");
                }

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

        private async Task<DoorEventType> DetermineDoorEventAsync(Door door, long userId)
        {
            var userHasAccessToDoors = await _doorAccessRepository.CanAccessAsync(userId, door.Id);

            if (!userHasAccessToDoors)
            {
                return DoorEventType.AccessDenied;
            }

            if (door.IsDeactivated)
            {
                return DoorEventType.DeactivatedDoorAccessAttempt;
            }

            return DoorEventType.AccessGranted;
        }
    }

    
}