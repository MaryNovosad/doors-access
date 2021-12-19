using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DoorsAccess.Models;

namespace DoorsAccess.DAL
{
    public class DoorAccessRepository : IDoorAccessRepository
    {
        private readonly string _connectionString;

        public DoorAccessRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<bool> CanAccessAsync(long userId, long doorId)
        {
            await using var connection = new SqlConnection(_connectionString);

            var query = @"SELECT COUNT(1) FROM [door_access]
                          WHERE [DoorId] = @DoorId AND [UserId] = @UserId AND [IsDeactivated] = @IsDeactivated";

            return await connection.ExecuteScalarAsync<bool>(query, new { userId, doorId, IsDeactivated = false });
        }

        public async Task<IList<DoorAccess>> GetAsync(long doorId)
        {
            await using var connection = new SqlConnection(_connectionString);

            var query = @"SELECT [DoorId], [UserId], [IsDeactivated], [IsOwner], [CreatedAt], [UpdatedAt] FROM [door_access]
                          WHERE [DoorId] = @DoorId";

            return (await connection.QueryAsync<DoorAccess>(query, new { doorId })).ToList();
        }

        public async Task CreateAsync(IList<DoorAccess> accesses)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var transaction = connection.BeginTransaction();

            var command = @"INSERT INTO [door_access] ([DoorId], [UserId], [IsDeactivated], [IsOwner],[CreatedAt], [UpdatedAt]) 
                            VALUES (@DoorId, @UserId, @IsDeactivated, @IsOwner, @CreatedAt, @UpdatedAt)";

            await connection.ExecuteAsync(command, accesses, transaction);

            transaction.Commit();
        }

        public async Task UpdateAsync(IList<DoorAccess> accesses)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var transaction = connection.BeginTransaction();

            var command = @"UPDATE [door_access] SET [IsDeactivated] = @IsDeactivated, [IsOwner] = @IsOwner, [UpdatedAt] = @UpdatedAt]
                            WHERE [DoorId] = @DoorId AND [UserId]";

            await connection.ExecuteAsync(command, accesses, transaction);

            transaction.Commit();
        }
    }
}
