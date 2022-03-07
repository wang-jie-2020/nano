using System.Net;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
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
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

builder.Configuration.AddJsonFile("./Configurations/ocelot.json");
builder.Services.AddOcelot(builder.Configuration).AddConsul().AddPolly();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.RoutePrefix = "swagger";
    c.SwaggerEndpoint("/CustomerService/swagger/v1/swagger.json", "Customer API");
    c.SwaggerEndpoint("/ProductService/swagger/v1/swagger.json", "Product API");
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
