using DoorsAccess.DAL;
using DoorsAccess.Models;

namespace DoorsAccess.Domain;

public class DoorsAccessHistoryService : IDoorsAccessHistoryService
{
    private readonly IDoorEventLogRepository _doorEventLogRepository;

    public DoorsAccessHistoryService(IDoorEventLogRepository doorEventLogRepository)
    {
        _doorEventLogRepository = doorEventLogRepository ?? throw new ArgumentNullException(nameof(doorEventLogRepository));
    }

    public async Task<IList<DetailedDoorEventLog>> GetDoorAccessHistoryAsync(long userId)
    {
        var logs = await _doorEventLogRepository.GetAsync(userId); 

        return logs;
    }

    public async Task<IList<DetailedDoorEventLog>> GetDoorAccessHistoryAsync()
    {
        var logs = await _doorEventLogRepository.GetAllAsync();

        return logs;
    }
}