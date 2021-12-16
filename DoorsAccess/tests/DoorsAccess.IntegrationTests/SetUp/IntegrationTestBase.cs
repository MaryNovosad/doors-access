using DoorsAccess.API;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace DoorsAccess.IntegrationTests.SetUp
{
    public class IntegrationTestBase
    {
        private readonly IConfiguration _config;
        private readonly TestServer _testServer;
        private readonly DbInstaller _dbInstaller;

        protected const long TestDoorId = 1;

        protected const long TestUserId = 2;
        protected const string TestUserRole = "User";

        protected const long TestAdminId = 2;
        protected const string TestAdminRole = "Admin";

        public IntegrationTestBase()
        {
            _config = new ConfigurationManager().AddJsonFile("appsettings.json").Build();

            _dbInstaller = new DbInstaller(_config.GetConnectionString("DoorsAccessDb"));
            _dbInstaller.SetUp();

            _testServer = CreateServerWithTestAuth();
        }

        [SetUp]
        public async Task Setup()
        {
            await _dbInstaller.ResetAsync();
        }

        private TestServer CreateServerWithTestAuth()
        {
            var builder = new WebHostBuilder()
                .UseKestrel()
                .UseConfiguration(_config)
                .UseTestServer()
                .UseStartup<Startup>()
                .ConfigureTestServices(services =>
                {
                    services
                        .AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
                }); 

            return new TestServer(builder);
        }

        protected HttpClient CreateHttpClient(long userId = TestUserId, string role = TestUserRole)
        {
            var httpClient = _testServer.CreateClient();

            httpClient.DefaultRequestHeaders.Add(TestHttpHeaders.UserId, userId.ToString());
            httpClient.DefaultRequestHeaders.Add(TestHttpHeaders.Role, role);

            return httpClient;
        }
    }
}
