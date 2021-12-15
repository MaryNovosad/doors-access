using DoorsAccess.DAL;

namespace DoorsAccess.Domain
{
    public interface IDoorsAccessHistoryService
    {
        Task<IList<DoorEventLog>> GetDoorAccessHistoryAsync(long userId);
        Task<IList<DoorEventLog>> GetDoorAccessHistoryAsync();
    }
}
