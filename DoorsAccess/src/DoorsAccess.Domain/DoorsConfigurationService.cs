using DoorsAccess.DAL;
using DoorsAccess.DAL.Repositories;
using DoorsAccess.Domain.DTOs;

namespace DoorsAccess.Domain;

public class DoorsConfigurationService: IDoorsConfigurationService
{
    private readonly IDoorRepository _doorRepository;

    public DoorsConfigurationService(IDoorRepository doorRepository)
    {
        _doorRepository = doorRepository;
    }

    public async Task CreateOrUpdateDoorAsync(DoorInfo doorInfo)
    {
        var existingDoor = await _doorRepository.GetAsync(doorInfo.Id);
        var utcDateTime = DateTime.UtcNow;

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
        }
    }

    public async Task<Door?> GetDoorAsync(long doorId)
    {
        return await _doorRepository.GetAsync(doorId);
    }

    private bool DoorInfoHasChanged(Door existingDoor, DoorInfo doorInfo) =>
        existingDoor.Name != doorInfo.Name || existingDoor.IsDeactivated != doorInfo.IsDeactivated;
}