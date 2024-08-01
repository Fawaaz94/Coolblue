using System.Threading;
using System.Threading.Tasks;
using Application.Surcharges.Commands;
using Application.Surcharges.Queries;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Insurance.Api.Controllers;

public class AdminController : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/surcharges")
            .WithTags("Admin");
        
        group.MapGet("/get", ListSurchargeRates);
        group.MapPost("/create", CreateSurchargeRates);
        group.MapPut("/update/{id}", UpdateSurchargeRates);
    }

    private static Task<SurchargeRateVm> ListSurchargeRates([FromServices] ISender sender)
    {
        return sender.Send(new GetSurchargeRatesQuery());
    }

    private static Task<int> CreateSurchargeRates(
        [FromServices] ISender sender,
        CreateSurchargeCommand command,
        CancellationToken cancellationToken)
    {
        return sender.Send(command, cancellationToken);
    }
    
    private static async Task<IResult> UpdateSurchargeRates(
        [FromServices] ISender sender,
        UpdateSurchargeCommand command,
        int id,
        CancellationToken cancellationToken)
    {
        if (id != command.Id) return Results.BadRequest();
        await sender.Send(command, cancellationToken);
        return Results.NoContent();
    }
}