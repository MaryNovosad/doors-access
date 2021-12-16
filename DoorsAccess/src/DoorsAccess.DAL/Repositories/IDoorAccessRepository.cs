using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DoorsAccess.DAL.Repositories
{
    public interface IDoorAccessRepository
    {
        Task<bool> CanAccessAsync(long userId, long doorId);
        Task<IList<DoorAccess>> GetAsync(long doorId);

        Task CreateAsync(IList<DoorAccess> accesses);

        Task UpdateAsync(IList<DoorAccess> accesses);
    }
}