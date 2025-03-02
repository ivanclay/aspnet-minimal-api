using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;
using RangoAgil.API.Models;

namespace RangoAgil.API.EndPoitHandlers;

public static class IngredientesHandlers
{
    public static async Task<Results<NotFound, Ok<IEnumerable<IngredienteDTO>>>> GetIngredientesByRangoIdAsync
    (RangoDbContext context,
     IMapper mapper,
    int rangoId)
    {
        var rangoEntity = await context
                                    .Rangos
                                    .FirstOrDefaultAsync(rango => rango.Id == rangoId);

        if (rangoEntity == null)
            return TypedResults.NotFound();

        return TypedResults.Ok(mapper.Map<IEnumerable<IngredienteDTO>>((await context
                           .Rangos
                           .Include(rango => rango.Ingredientes)
                           .FirstOrDefaultAsync(rango => rango.Id == rangoId))?.Ingredientes));
    }
}
