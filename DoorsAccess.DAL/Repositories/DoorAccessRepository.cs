using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace DoorsAccess.DAL.Repositories
{
    public class DoorAccessRepository : IDoorAccessRepository
    {
        private readonly string _connectionString;

        public DoorAccessRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<bool> CanAccess(long userId, long doorId, DateTime date)
        {
            await using var connection = new SqlConnection(_connectionString);

            var query = @"SELECT COUNT(1) FROM [door_access]
                          WHERE [DoorId] = @DoorId AND [UserId] = @UserId AND [FromDate] < @Date AND ([EndDate] IS NULL OR @Date < [EndDate]";

            return await connection.ExecuteScalarAsync<bool>(query, new { userId, doorId, date });
        }
    }
}
