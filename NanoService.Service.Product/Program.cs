using Com.Ctrip.Framework.Apollo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Routing;
using NanoService.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

//builder.Configuration.AddApollo(builder.Configuration.GetSection("apollo"))
//    .AddDefault()
//    .AddNamespace("nano-product");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(option =>
   {
       option.Authority = builder.Configuration["auth.authority"];
       option.Audience = builder.Configuration["auth.audience"];
       option.RequireHttpsMetadata = false;
   });

builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();

//app.UseSwagger(c => c.PreSerializeFilters.Add(
//    (doc, req) =>
//    {
//        OpenApiPaths paths = new OpenApiPaths();

//        foreach (var path in doc.Paths)
//        {
//            paths.Add("/ProductService" + path.Key, path.Value);
//        }

//        doc.Paths = paths;
//    }));

app.UseSwagger();
app.UseSwaggerUI();
app.UseHealthChecks("/healthz");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//app.Map("/apollo", b =>
//{
//    b.Run(async handler =>
//    {
//        var result = app.Configuration["product.health"];
//        await handler.Response.WriteAsync(result);
//    });
//});


app.UseConsul(app.Configuration);

app.Run();