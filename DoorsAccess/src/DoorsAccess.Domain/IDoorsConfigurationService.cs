using DoorsAccess.Domain.DTO;
using DoorsAccess.Models;

namespace DoorsAccess.Domain
{
    public interface IDoorsConfigurationService
    {
        Task CreateOrUpdateDoorAsync(DoorInfo door);
        Task ChangeActivationStateAsync(long doorId, bool isDeactivated);
        Task<Door?> GetDoorAsync(long doorId);
    }
}
