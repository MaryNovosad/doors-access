using DoorsAccess.DAL;
using DoorsAccess.DAL.Repositories;
using DoorsAccess.Domain.DTO;
using DoorsAccess.Domain.Utils;
using Microsoft.Extensions.Logging;

namespace DoorsAccess.Domain;

public class DoorsConfigurationService : IDoorsConfigurationService
{
    private readonly IDoorRepository _doorRepository;
    private readonly IClock _clock;
    private readonly ILogger<DoorsConfigurationService> _logger;

    public DoorsConfigurationService(IDoorRepository doorRepository, IClock clock, ILogger<DoorsConfigurationService> logger)
    {
        _doorRepository = doorRepository;
        _clock = clock;
        _logger = logger;
    }

    public async Task CreateOrUpdateDoorAsync(DoorInfo doorInfo)
    {
        var existingDoor = await _doorRepository.GetAsync(doorInfo.Id);
        var utcDateTime = _clock.UtcNow();

        if (existingDoor != null)
        {
            if (DoorInfoHasChanged(existingDoor, doorInfo))
            {
                var updatedDoor = new Door
                {
                    Id = doorInfo.Id,
                    Name = doorInfo.Name,
                    IsDeactivated = doorInfo.IsDeactivated,
                    CreatedAt = existingDoor.CreatedAt,
                    UpdatedAt = utcDateTime
                };

                await _doorRepository.UpdateAsync(updatedDoor);

                _logger.LogInformation($"Door {updatedDoor.Id} is updated");
            }
        }
        else
        {
            var newDoor = new Door
            {
                Id = doorInfo.Id,
                Name = doorInfo.Name,
                IsDeactivated = doorInfo.IsDeactivated,
                CreatedAt = utcDateTime,
                UpdatedAt = utcDateTime
            };

            await _doorRepository.CreateAsync(newDoor);

            _logger.LogInformation($"Door {newDoor.Id} is created");
        }
    }

    public async Task ChangeActivationStateAsync(long doorId, bool isActivated)
    {
        await _doorRepository.ChangeActivationStateAsync(doorId, isActivated);

        _logger.LogInformation($"Door {doorId} is {(isActivated ? "" : "de")}activated");
    }

    public async Task<Door?> GetDoorAsync(long doorId)
    {
        return await _doorRepository.GetAsync(doorId);
    }

    private bool DoorInfoHasChanged(Door existingDoor, DoorInfo doorInfo) =>
        existingDoor.Name != doorInfo.Name || existingDoor.IsDeactivated != doorInfo.IsDeactivated;
}