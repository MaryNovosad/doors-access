using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DoorsAccess.Models;

namespace DoorsAccess.DAL
{
    public class DoorEventLogRepository : IDoorEventLogRepository
    {
        private readonly string _connectionString;

        public DoorEventLogRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task AddAsync(DoorEventLog doorEventLog)
        {
            await using var connection = new SqlConnection(_connectionString);

            var command = "INSERT INTO [door_event_log] ([DoorId], [UserId], [TimeStamp], [EventType]) VALUES (@DoorId, @UserId, @TimeStamp, @EventType)";
            await connection.ExecuteAsync(command, doorEventLog);
        }

        public async Task<IList<DetailedDoorEventLog>> GetAllAsync()
        {
            await using var connection = new SqlConnection(_connectionString);

            var query = @"SELECT del.[DoorId], d.[Name] as [DoorName], d.[IsDeactivated] as IsDoorDeactivated, del.[UserId], del.[TimeStamp], del.[EventType]
                          FROM [door_event_log] del
                          JOIN [door] d ON del.[DoorId] = d.[Id]
                          ORDER BY [TimeStamp]";

            return (await connection.QueryAsync<DetailedDoorEventLog>(query)).ToList();
        }

        public async Task<IList<DetailedDoorEventLog>> GetAsync(long userId)
        {
            await using var connection = new SqlConnection(_connectionString);

            var query = @"SELECT del.[DoorId], d.[Name] as [DoorName], d.[IsDeactivated] as IsDoorDeactivated, del.[UserId], del.[TimeStamp], del.[EventType]
                          FROM [door_event_log] del
                          JOIN [door] d ON del.[DoorId] = d.[Id]
                          WHERE [UserId] = @UserId
                          ORDER BY [TimeStamp]";

            return (await connection.QueryAsync<DetailedDoorEventLog>(query, new { userId })).ToList();
        }
    }
}
