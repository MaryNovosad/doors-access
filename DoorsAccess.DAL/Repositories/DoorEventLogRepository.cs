using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace DoorsAccess.DAL.Repositories
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

            var command = "INSERT INTO [door_event_log] ([DoorId], [UserId], [TimeStamp], [Event]) VALUES (@DoorId, @UserId, @TimeStamp, @Event)";
            await connection.ExecuteAsync(command, new { doorEventLog });
        }

        // time range param?
        public async Task<IList<DoorEventLog>> GetAllAsync()
        {
            await using var connection = new SqlConnection(_connectionString);

            var query = "SELECT [DoorId], [UserId], [TimeStamp], [Event] FROM [door_event_log]";

            return (await connection.QueryAsync<DoorEventLog>(query)).ToList();
        }

        // time range param?
        public async Task<IList<DoorEventLog>> GetAsync(long userId)
        {
            await using var connection = new SqlConnection(_connectionString);

            var query = "SELECT [DoorId], [UserId], [TimeStamp], [Event] FROM [door_event_log] WHERE [UserId] = @UserId";

            return (await connection.QueryAsync<DoorEventLog>(query, new { userId })).ToList();
        }
    }
}
