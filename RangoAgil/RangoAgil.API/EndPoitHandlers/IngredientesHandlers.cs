using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;
using RangoAgil.API.Models;

namespace RangoAgil.API.EndPoitHandlers;

public static class IngredientesHandlers
{
    public static async Task<IEnumerable<IngredienteDTO>> GetIngredientesByRangoId
    (RangoDbContext context,
     IMapper mapper,
    int rangoId) 
{
    return mapper.Map<IEnumerable<IngredienteDTO>>((await context
                    .Rangos
                    .Include(rango => rango.Ingredientes)
                    .FirstOrDefaultAsync(rango => rango.Id == rangoId))?.Ingredientes);
}
}
