using System.Reflection;
using Application.Interfaces;
using Application.Services;
using Domain.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
            ;
            
        services.AddHttpClient(CommonConstants.InsuranceApi, client =>
        {
            client.BaseAddress = new Uri(configuration["ProductApiAddress"] ?? throw new InvalidOperationException());
        });
        
        services.AddScoped<IInsuranceServices, InsuranceServices>();
        services.AddScoped<IInsuranceCalculator, InsuranceCalculator>();
        
        return services;
    }
}