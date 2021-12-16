using IdentityServer4.Configuration;
using UserIdentity.API;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddIdentityServer(options => options.UserInteraction = GetUserInteractionOptions())
    .AddInMemoryClients(IdentityConfiguration.Clients)
    .AddInMemoryIdentityResources(IdentityConfiguration.IdentityResources)
    .AddInMemoryApiScopes(IdentityConfiguration.ApiScopes)
    .AddTestUsers(IdentityConfiguration.TestUsers.ToList())
    .AddDeveloperSigningCredential();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();

app.UseIdentityServer();
app.UseAuthorization();
app.UseEndpoints(e => e.MapControllerRoute(
    name: "default",
    pattern: "{action=Login}",
    defaults: new { controller = "Home" }));

app.Run();

UserInteractionOptions GetUserInteractionOptions()
{
    return new UserInteractionOptions
    {
        LoginUrl = "/Login",
        LogoutUrl = "/Logout"
    };
}


