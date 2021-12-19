using System.Threading.Tasks;
using DoorsAccess.Models;
using DoorsAccess.Models.Enums;

namespace DoorsAccess.DAL.Repositories
{
    public interface IDoorRepository
    {
        Task<Door?> GetAsync(long doorId);
        Task CreateAsync(Door door);
        Task UpdateAsync(Door door);
        Task ChangeStateAsync(long doorId, DoorState state);
        Task ChangeActivationStateAsync(long doorId, bool isActivated);
    }
}