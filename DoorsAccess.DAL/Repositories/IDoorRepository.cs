using System.Threading.Tasks;

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