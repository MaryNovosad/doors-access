using DoorsAccess.DAL;

namespace DoorsAccess.Domain
{
    public interface IDoorsAccessHistoryService
    {
        Task<IList<DetailedDoorEventLog>> GetDoorAccessHistoryAsync(long userId);
        Task<IList<DetailedDoorEventLog>> GetDoorAccessHistoryAsync();
    }
}
