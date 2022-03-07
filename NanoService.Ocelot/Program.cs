using System.Net;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.OpenApi.Models;
using NanoService.Infrastructure.Extensions;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.Provider.Polly;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

Log.Logger = new LoggerConfiguration()

    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}"
        , theme: AnsiConsoleTheme.Code)
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("System", LogEventLevel.Information)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows()
        {
            Password = new OpenApiOAuthFlow()
            {
                AuthorizationUrl = new Uri("http://vm.local.cn:5501/connect/authorize"),
                Scopes = new Dictionary<string, string>
                {
                    {"customer.scope", "customer的scope"},
                    {"product.scope", "product的scope"}

                },
                TokenUrl = new Uri("http://vm.local.cn:5501/connect/token"),
            }
        }
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "oauth2"
                        }
                    },
                    Array.Empty<string>()
                }
            });
});

builder.Services.AddHealthChecks();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("ProductServiceAuthentication", option =>
    {
        option.Authority = "http://vm.local.cn:5501";
        option.Audience = "admin.resource";
        option.RequireHttpsMetadata = false;
    })
    .AddJwtBearer("CustomerServiceAuthentication", option =>
    {
        option.Authority = "http://vm.local.cn:5501";
        option.Audience = "admin.resource";
        option.RequireHttpsMetadata = false;
    });


builder.Configuration.AddJsonFile("./Configurations/ocelot.json");
builder.Services.AddOcelot(builder.Configuration).AddConsul().AddPolly();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.RoutePrefix = "swagger";
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ocelot API");
    c.SwaggerEndpoint("/CustomerService/swagger/v1/swagger.json", "Customer API");
    c.SwaggerEndpoint("/ProductService/swagger/v1/swagger.json", "Product API");

    c.OAuthClientId("client_resourceOwnerPassword");
    c.OAuthClientSecret("secret");
    c.OAuthUsePkce();
});

/*
 *  在非路由项目中运行正常，但Ocelot中运行不正常
 */
//app.MapHealthChecks("/healthz");
/*
 *  在非路由项目中运行正常，但Ocelot中运行不正常
 */
//app.MapGet("/healthz", async handler =>
//{
//    await handler.Response.WriteAsync("healthy");
//});

app.Map("/healthz", b =>
{
    b.Run(async handler =>
    {
        await handler.Response.WriteAsync("ok");
    });
});

app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();

app.UseConsul(app.Configuration);
app.UseOcelot().Wait();

app.Run();
