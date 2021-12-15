using System.Collections.Generic;
using System.Threading.Tasks;

namespace DoorsAccess.DAL.Repositories
{
    public interface IDoorEventLogRepository
    {
        Task<IList<DoorEventLog>> GetAsync(long userId);
        Task<IList<DoorEventLog>> GetAllAsync();
        Task AddAsync(DoorEventLog doorEventLog);
    }
}