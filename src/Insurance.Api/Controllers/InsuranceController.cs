using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Interfaces;
using Carter;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Insurance.Api.Controllers;

public class InsuranceController : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/insurance")
            .WithTags("Insurance");
        
        group.MapPost("/product/{id}", CalculateProductInsurance).WithName("CalculateProductInsurance");
        group.MapPost("/order", CalculateOrderInsurance).WithName("CalculateOrderInsurance");
    }
    
    public async Task<IResult> CalculateProductInsurance(
        [FromServices] IInsuranceServices insuranceServices, 
        int id)
    {
        var result = await insuranceServices.CalculateProductInsuranceAsync(id);
        return Results.Ok(result);
    }  
    
    public async Task<IResult> CalculateOrderInsurance(
        [FromServices] IInsuranceServices insuranceServices, 
        [FromBody] OrderInsuranceRequest request)
    {
        if (request.Products == null)
            return Results.BadRequest();
                
        var result = await insuranceServices.CalculateOrderInsuranceAsync(request.Products);
        return Results.Ok(result);
    }
}

public record OrderInsuranceRequest(IEnumerable<Product> Products);