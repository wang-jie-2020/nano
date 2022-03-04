using System.Security.Claims;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.IdentityModel.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddIdentityServer(options => { IdentityModelEventSource.ShowPII = true; })
    .AddInMemoryIdentityResources(Config.IdentityResources())
    .AddInMemoryApiScopes(Config.ApiScopes())
    .AddInMemoryApiResources(Config.ApiResources())
    .AddInMemoryClients(Config.Clients())
    .AddTestUsers(Config.Users())
    //.AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
    .AddDeveloperSigningCredential();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.UseIdentityServer();

app.Run();

static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources()
    {
        return new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile()
        };
    }

    public static IEnumerable<ApiScope> ApiScopes()
    {
        return new List<ApiScope>
        {
            new ApiScope(name: "customer.scope", displayName: "customer.scope"),
            new ApiScope(name: "product.scope", displayName: "product.scope"),
        };
    }

    public static IEnumerable<ApiResource> ApiResources()
    {
        return new List<ApiResource>
        {
            new ApiResource(name: "customer.resource", displayName: "customer.resource")
            {
                Scopes = new string[]
                {
                    "customer.scope"
                }
            },
            new ApiResource(name: "product.resource", displayName: "product.resource")
            {
                Scopes = new string[]
                {
                    "product.scope"
                }
            },
        };
    }

    public static List<TestUser> Users()
    {
        return new List<TestUser>
        {
            new TestUser
            {
                SubjectId = "1",
                Username = "admin",
                Password = "admin",
                Claims = new[]
                {
                    new Claim("name", "admin"),
                    new Claim("role", "admin"),
                }
            },
            new TestUser
            {
                SubjectId = "2",
                Username = "operator",
                Password = "operator",
                Claims = new[]
                {
                    new Claim("name", "operator"),
                    new Claim("role", "operator"),
                }
            },
            new TestUser
            {
                SubjectId = "3",
                Username = "guest",
                Password = "guest",
                Claims = new[]
                {
                    new Claim("name", "guest"),
                    new Claim("role", "guest"),
                }
            }
        };
    }

    public static IEnumerable<Client> Clients()
    {
        return new List<Client>
        {
            new Client
            {
                ClientId = "client_Credential",
                ClientSecrets = {new Secret("secret".Sha256())},
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = { "customer.scope", "product.scope" }
            },

            new Client
            {
                ClientId = "client_resourceOwnerPassword",
                ClientSecrets = {new Secret("secret".Sha256())},
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                AllowedScopes = { "customer.scope", "product.scope" }
            }
        };
    }
}