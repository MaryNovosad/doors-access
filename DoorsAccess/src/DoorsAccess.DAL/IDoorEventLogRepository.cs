using System.Collections.Generic;
using System.Threading.Tasks;
using DoorsAccess.Models;

namespace DoorsAccess.DAL
{
    public interface IDoorEventLogRepository
    {
        Task<IList<DetailedDoorEventLog>> GetAsync(long userId);
        Task<IList<DetailedDoorEventLog>> GetAllAsync();
        Task AddAsync(DoorEventLog doorEventLog);
    }
}