using Microsoft.OpenApi.Models;
using NanoService.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseSwagger(c => c.PreSerializeFilters.Add(
    (doc, req) =>
    {
        OpenApiPaths paths = new OpenApiPaths();

        foreach (var path in doc.Paths)
        {
            paths.Add("/ProductService" + path.Key, path.Value);
        }

        doc.Paths = paths;
    }));

app.UseSwaggerUI();
app.UseHealthChecks("/healthz");

app.UseAuthorization();

app.MapControllers();

app.UseConsul(app.Configuration);

app.Run();