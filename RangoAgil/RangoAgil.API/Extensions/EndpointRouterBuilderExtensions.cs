using RangoAgil.API.EndpointFilters;
using RangoAgil.API.EndPoitHandlers;

namespace RangoAgil.API.Extensions;

public static class EndpointRouterBuilderExtensions
{
    public static void RegisterRangosEndpoints(this IEndpointRouteBuilder endpointRoutebuilder)
    {
        var rangosEndpoints = endpointRoutebuilder.MapGroup("/rangos");
        var rangosComIdEndpoints = rangosEndpoints.MapGroup("/{rangoId:int}");

        var rangosComIdAndLockFilterEndpoints = 
            endpointRoutebuilder.MapGroup("/rangos/{rangoId:int}")
                                .AddEndpointFilter(new RangoIsLockedFilter(6))
                                .AddEndpointFilter(new RangoIsLockedFilter(17));

        rangosEndpoints.MapGet("", RangosHandlers.GetRangosAsync);

        rangosEndpoints.MapPost("", RangosHandlers.CreateRangoAsync)
                       .AddEndpointFilter<ValidateAnnotationFilter>();

        rangosComIdEndpoints.MapGet("", RangosHandlers.GetRangoById)
                    .WithName("GetRangoById");

        rangosComIdAndLockFilterEndpoints.MapPut("", RangosHandlers.UpdateRangoAsync);

        rangosComIdAndLockFilterEndpoints.MapDelete("", RangosHandlers.DeleteRangoAsync)
                                         .AddEndpointFilter<LogNotFoundResponseFilter>();
    }

    public static void RegisterIngredientesEndpoints(this IEndpointRouteBuilder endpointRoutebuilder)
    {
        var rangosComIngredientesEndpoints = endpointRoutebuilder.MapGroup("rangos/{rangoId:int}/ingredientes");

        rangosComIngredientesEndpoints.MapGet("", IngredientesHandlers.GetIngredientesByRangoIdAsync);

    }
}
