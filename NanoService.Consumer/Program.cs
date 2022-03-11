using Exceptionless;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NanoService.Consumer.Models;
using NanoService.Infrastructure.Extensions;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;


var builder = WebApplication.CreateBuilder(args);

//DebugSerilog
Serilog.Debugging.SelfLog.Enable(Console.Error);

Log.Logger = new LoggerConfiguration()

    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}"
        , theme: AnsiConsoleTheme.Code)
    //.WriteTo.Exceptionless(builder.Configuration["Exceptionless:ApiKey"], builder.Configuration["Exceptionless:ServerUrl"], new[] { "nano" }, null, true, LogEventLevel.Information)
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("System", LogEventLevel.Information)
    .CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var sqlLiteConnection = new SqliteConnection("DataSource=:memory:");
sqlLiteConnection.Open();

builder.Services.AddEntityFrameworkSqlite().AddDbContext<SampleDbContext>(c => c.UseSqlite(sqlLiteConnection));

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();
app.UseHealthChecks("/healthz");

app.MapControllers();

app.UseConsul(app.Configuration);

app.Run();