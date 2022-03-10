using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NanoService.Consumer.Models;
using NanoService.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

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