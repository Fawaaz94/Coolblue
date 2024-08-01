using Application;
using Carter;
using Carter.ResponseNegotiators.Newtonsoft;
using Insurance.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructure();

builder.Services.AddHttpClient();
builder.Services.AddCarter(configurator: c =>
{
    c.WithResponseNegotiator<NewtonsoftJsonResponseNegotiator>();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Insurance API",
        Version = "v1"
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Api v1");
    });
}

app.UseHttpsRedirection();
app.UseDeveloperExceptionPage();

app.MapGet("/api/probe/health", () => "OK").ExcludeFromDescription();
app.MapCarter();
app.Run();