using System.Collections.Generic;
using System.Threading.Tasks;
using DoorsAccess.Models;

namespace DoorsAccess.DAL
{
    public interface IDoorAccessRepository
    {
        Task<bool> CanAccessAsync(long userId, long doorId);
        Task<IList<DoorAccess>> GetAsync(long doorId);

        Task CreateAsync(IList<DoorAccess> accesses);

        Task UpdateAsync(IList<DoorAccess> accesses);
    }
}