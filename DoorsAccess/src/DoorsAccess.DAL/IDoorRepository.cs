using System.Threading.Tasks;
using DoorsAccess.Models;
using DoorsAccess.Models.Enums;

namespace DoorsAccess.DAL
{
    public interface IDoorRepository
    {
        Task<Door?> GetAsync(long doorId);
        Task CreateAsync(Door door);
        Task UpdateAsync(Door door);
        Task ChangeStateAsync(long doorId, DoorState state);
        Task ChangeActivationStateAsync(long doorId, bool isDeactivated);
    }
}