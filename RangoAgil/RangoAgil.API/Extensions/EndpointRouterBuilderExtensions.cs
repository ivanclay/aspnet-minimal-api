using RangoAgil.API.EndPoitHandlers;

namespace RangoAgil.API.Extensions;

public static class EndpointRouterBuilderExtensions
{
    public static void RegisterRangosEndpoints(this IEndpointRouteBuilder endpointRoutebuilder)
    {
        var rangosEndpoints = endpointRoutebuilder.MapGroup("/rangos");
        var rangosComIdEndpoints = rangosEndpoints.MapGroup("/{rangoId:int}");

        rangosEndpoints.MapGet("", RangosHandlers.GetRangosAsync);

        rangosEndpoints.MapPost("", RangosHandlers.CreateRangoAsync);

        rangosComIdEndpoints.MapGet("", RangosHandlers.GetRangoById)
                            .WithName("GetRangoById");

        rangosComIdEndpoints.MapPut("", RangosHandlers.UpdateRangoAsync);

        rangosComIdEndpoints.MapDelete("", RangosHandlers.DeleteRangoAsync);
    }

    public static void RegisterIngredientesEndpoints(this IEndpointRouteBuilder endpointRoutebuilder)
    {
        var rangosComIngredientesEndpoints = endpointRoutebuilder.MapGroup("rangos/{rangoId:int}/ingredientes");

        rangosComIngredientesEndpoints.MapGet("", IngredientesHandlers.GetIngredientesByRangoIdAsync);

    }
}
