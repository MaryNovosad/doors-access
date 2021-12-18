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

        protected HttpClient CreateHttpClient(long userId = TestConstants.TestUserId, string role = TestConstants.TestUserRole)
        {
            var httpClient = _testServer.CreateClient();

            httpClient.DefaultRequestHeaders.Add(TestHttpHeaders.UserId, userId.ToString());
            httpClient.DefaultRequestHeaders.Add(TestHttpHeaders.Role, role);

            return httpClient;
        }
    }
}
