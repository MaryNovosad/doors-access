using System.Security.Claims;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace UserIdentity.API;

internal class IdentityConfiguration
{
    public static IEnumerable<Client> Clients =>
        new List<Client>
        {
            new ()
            {
                ClientId = "Clay",
                AllowedGrantTypes = GrantTypes.Code,

                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },

                RedirectUris = { "https://localhost:5001/signin-oidc" },
                PostLogoutRedirectUris = { "https://localhost:5001/signout-callback-oidc" },

                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "DoorAccessAPI"
                }
            }
        };

    public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile()
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        {
            new ("DoorAccessAPI", "Door Access API", new List<string>{ JwtClaimTypes.Role })
        };

    public static IEnumerable<TestUser> TestUsers =>
        new List<TestUser>
        {
            new()
            {
                SubjectId = "1",
                Username = "Bob",
                Password = "BobPassword",
                Claims =
                {
                    new Claim(JwtClaimTypes.Name, "Bob Smith"),
                    new Claim(JwtClaimTypes.Email, "BobSmith@email.com"),
                    new Claim(JwtClaimTypes.Role, "Admin")
                }
            },
            new()
            {
                SubjectId = "2",
                Username = "Mary",
                Password = "MaryPassword",
                Claims =
                {
                    new Claim(JwtClaimTypes.Name, "Mary Novosad"),
                    new Claim(JwtClaimTypes.Email, "mary@email.com"),
                    new Claim(JwtClaimTypes.Role, "User")
                }
            }
        };
}