using DbUp;
using DbUp.ScriptProviders;
using Respawn;

namespace DoorsAccess.IntegrationTests.SetUp
{
    public class DbInstaller
    {
        private readonly string _connectionString;

        public DbInstaller(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void SetUp()
        {
            CreateDbIfNotExists();
            RunMigrationScripts(@"..\..\..\..\DoorsAccess.Database", _connectionString);
        }

        public async Task ResetAsync()
        {
            var dbCheckpoint = new Checkpoint();
            await dbCheckpoint.Reset(_connectionString);
        }

        private void CreateDbIfNotExists()
        {
            EnsureDatabase.For.SqlDatabase(_connectionString);
        }

        private void RunMigrationScripts(string scriptsPath, string connectionString)
        {
            var fullPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, scriptsPath));

            var builder = DeployChanges.To
                                       .SqlDatabase(connectionString)
                                       .WithScriptsFromFileSystem(fullPath, new FileSystemScriptOptions() { IncludeSubDirectories = true })
                                       .WithTransactionPerScript()
                                       .LogToConsole();

            var engine = builder.Build();

            var result = engine.PerformUpgrade();

            if (!result.Successful)
            {
                throw result.Error;
            }
        }
    }
}
