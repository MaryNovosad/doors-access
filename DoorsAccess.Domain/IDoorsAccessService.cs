using DoorsAccess.DAL;

namespace DoorsAccess.Domain
{
    public interface IDoorsAccessService
    {
        Task GrantDoorAccessAsync(long doorId, long userId);
    }
}