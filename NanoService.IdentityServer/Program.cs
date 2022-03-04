using System.Security.Claims;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.IdentityModel.Logging;
using NanoService.IdentityServer;
using NanoService.Infrastructure.Extensions;

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

app.UseConsul(app.Configuration);

app.Run();