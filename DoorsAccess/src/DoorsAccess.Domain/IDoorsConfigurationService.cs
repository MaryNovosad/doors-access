using DoorsAccess.DAL;
using DoorsAccess.Domain.DTOs;

namespace DoorsAccess.Domain
{
    public interface IDoorsConfigurationService
    {
        Task CreateOrUpdateDoorAsync(DoorInfo door);
        Task<Door?> GetDoorAsync(long doorId);
    }
}
