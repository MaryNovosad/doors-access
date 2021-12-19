using Azure.Messaging.ServiceBus;
using DoorsAccess.DAL.Repositories;
using DoorsAccess.Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DoorsAccess.API.Infrastructure;
using DoorsAccess.Domain.Utils;
using DoorsAccess.Messaging;
using Microsoft.IdentityModel.Tokens;

namespace DoorsAccess.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IDoorsAccessService, DoorsAccessService>();
            services.AddTransient<IDoorsConfigurationService, DoorsConfigurationService>();
            services.AddTransient<IDoorsAccessHistoryService, DoorsAccessHistoryService>();

            var serviceBusOptions = new DoorAccessServiceBusSenderOptions();
            Configuration.GetSection("Messaging:ServiceBus").Bind(serviceBusOptions);

            services.AddSingleton(_ => new ServiceBusClient(serviceBusOptions.ConnectionString));

            services.AddSingleton<IDoorAccessMessageSender, DoorAccessServiceBusSender>();

            var dbConnectionString = Configuration.GetConnectionString("DoorsAccessDb");

            services.AddTransient<IDoorRepository>(_ => new DoorRepository(dbConnectionString));
            services.AddTransient<IDoorAccessRepository>(_ => new DoorAccessRepository(dbConnectionString));
            services.AddTransient<IDoorEventLogRepository>(_ => new DoorEventLogRepository(dbConnectionString));

            services.AddSingleton<IClock, Clock>();

            var authConfiguration = Configuration.GetSection("Authentication");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = authConfiguration.GetValue<string>("IdentityServerUrl");
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false
                    };
                });

            services.AddLogging();
            services.AddControllers();
            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.ConfigureExceptionMiddleware();

            app.UseHsts();

            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
