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
builder.Services.AddHealthChecks();

builder.Services.AddIdentityServer(options => { IdentityModelEventSource.ShowPII = true; })
    .AddInMemoryIdentityResources(Config.IdentityResources())
    .AddInMemoryApiScopes(Config.ApiScopes())
    .AddInMemoryApiResources(Config.ApiResources())
    .AddInMemoryClients(Config.Clients())
    .AddTestUsers(Config.Users())
    //.AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
    .AddDeveloperSigningCredential();

builder.Services.AddCors(options =>
{
    options.AddPolicy("default", policy =>
     {
         policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
     });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHealthChecks("/healthz");

app.UseAuthorization();

app.MapControllers();

app.UseCors("default");

app.UseIdentityServer();

app.UseConsul(app.Configuration);

app.Run();