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
            services.AddTransient<IIoTDeviceProxy, IoTDeviceProxyMock>();
            services.AddTransient<IDoorRepository>(sp => new DoorRepository(Configuration.GetConnectionString("DoorsAccessDb")));
            services.AddTransient<IDoorAccessRepository>(sp => new DoorAccessRepository(Configuration.GetConnectionString("DoorsAccessDb")));
            services.AddTransient<IDoorEventLogRepository>(sp => new DoorEventLogRepository(Configuration.GetConnectionString("DoorsAccessDb")));

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
                app.UseExceptionHandler(new ExceptionHandlerOptions
                {
                    ExceptionHandlingPath = "/Error",
                    AllowStatusCode404Response = true
                });
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
