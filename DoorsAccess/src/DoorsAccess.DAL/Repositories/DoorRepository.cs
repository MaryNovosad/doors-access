using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace DoorsAccess.DAL.Repositories
{
    public class DoorRepository : IDoorRepository
    {
        private readonly string _connectionString;

        public DoorRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task CreateAsync(Door door)
        {
            await using var connection = new SqlConnection(_connectionString);

            var command = @"INSERT INTO [door] ([Id], [Name], [State], [UpdatedAt], [CreatedAt], [IsDeactivated])
                            VALUES (@Id, @Name, @State, @UpdatedAt, @CreatedAt, @IsDeactivated)";
            await connection.ExecuteAsync(command, door);
        }

        public async Task UpdateAsync(Door door)
        {
            await using var connection = new SqlConnection(_connectionString);

            var command = "UPDATE [door] SET [Name] = @Name, [State] = @State, [UpdatedAt] = @UpdatedAt, [IsDeactivated] = @IsDeactivated WHERE [Id] = @Id";
            await connection.ExecuteAsync(command, door);
        }

        public async Task ChangeStateAsync(long doorId, DoorState state)
        {
            await using var connection = new SqlConnection(_connectionString);

            var command = "UPDATE [door] SET [State] = @State, [UpdatedAt] = GETDATE() WHERE [Id] = @DoorId";
            await connection.ExecuteAsync(command, new { doorId, state });
        }

        public async Task ChangeActivationStateAsync(long doorId, bool isActivated)
        {
            await using var connection = new SqlConnection(_connectionString);

            var command = "UPDATE [door] SET [IsDeactivated] = @IsDeactivated, [UpdatedAt] = GETDATE() WHERE [Id] = @DoorId";
            await connection.ExecuteAsync(command, new { doorId, IsDeactivated = isActivated });
        }

        public async Task<Door?> GetAsync(long doorId)
        {
            await using var connection = new SqlConnection(_connectionString);

            var query = "SELECT [Id], [Name], [State], [UpdatedAt], [IsDeactivated] FROM [door] WHERE [Id] = @DoorId";

            return await connection.QueryFirstOrDefaultAsync<Door>(query, new { doorId }); 
        }
    }
}
