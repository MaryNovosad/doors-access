using IdentityServer4.Models;
using IdentityServer4.Test;

namespace UserIdentity.API;

internal class IdentityConfiguration
{
    public static IEnumerable<Client> Clients { get; set; }
    public static IEnumerable<IdentityResource> IdentityResources { get; set; }
    public static IEnumerable<ApiResource> ApiResources { get; set; }
    public static IEnumerable<ApiScope> ApiScopes { get; set; }
    public static List<TestUser> TestUsers { get; set; }
}