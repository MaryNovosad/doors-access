using System.Net.Mime;
using System.Threading.Tasks;
using DoorsAccess.DAL.Repositories;
using DoorsAccess.Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DoorsAccess.API.Infrastructure;
using DoorsAccess.IoT.Integration;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

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

            services.AddTransient<IIoTDeviceProxy, IoTDeviceProxyMock>();

            var dbConnectionString = Configuration.GetConnectionString("DoorsAccessDb");

            services.AddTransient<IDoorRepository>(_ => new DoorRepository(dbConnectionString));
            services.AddTransient<IDoorAccessRepository>(_ => new DoorAccessRepository(dbConnectionString));
            services.AddTransient<IDoorEventLogRepository>(_ => new DoorEventLogRepository(dbConnectionString));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);

            services.AddControllers(options =>
            {
                options.Filters.Add<HttpResponseExceptionFilter>();
            });

            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(c => c.Run(async context =>
                {
                    var exception = context.Features
                        .Get<IExceptionHandlerPathFeature>()
                        .Error;
                    var response = new { error = exception.Message };
                    await context.Response.WriteAsJsonAsync(response);
                }));
                app.UseHsts();
            }

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
