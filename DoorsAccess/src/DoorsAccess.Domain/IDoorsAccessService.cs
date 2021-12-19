namespace DoorsAccess.Domain
{
    public interface IDoorsAccessService
    {
        Task OpenDoorAsync(long doorId, long userId);
        Task AllowDoorAccessAsync(long doorId, IList<long> usersIds);
    }
}