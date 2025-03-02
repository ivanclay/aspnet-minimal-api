using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;
using RangoAgil.API.Entities;
using RangoAgil.API.Models;

namespace RangoAgil.API.EndPoitHandlers;

public static class RangosHandlers
{
    public static async Task<Results<NoContent, Ok<IEnumerable<RangoDTO>>>> GetRangosAsync
    (RangoDbContext context,
    IMapper mapper,
    [FromQuery(Name = "name")] string? rangoName)
    {
        var rangoEntity = await context
                            .Rangos
                            .Where(rango => rangoName == null || rango.Nome.ToLower().Contains(rangoName.ToLower()))
                            .ToListAsync();
        if (rangoEntity == null || rangoEntity.Count <= 0)
            return TypedResults.NoContent();
        return TypedResults.Ok(mapper.Map<IEnumerable<RangoDTO>>(rangoEntity));
    }

    public static async Task<CreatedAtRoute<RangoDTO>> PostRangoAsync(
    RangoDbContext context,
    IMapper mapper,
    [FromBody] RangoCreateDTO rangoCreateDTO)
    {
        var rangoEntity = mapper.Map<Rango>(rangoCreateDTO);
        context.Rangos.Add(rangoEntity);
        await context.SaveChangesAsync();

        var rangoReturn = mapper.Map<RangoDTO>(rangoEntity);

        return TypedResults.CreatedAtRoute(
                                rangoReturn,
                                "GetRangosById",
                                new
                                {
                                    rangoId = rangoReturn.Id
                                });
    }

    public static async Task<RangoDTO> GetRangoById(
    RangoDbContext context,
    IMapper mapper,
    int RangoId)
    {
        return mapper.Map<RangoDTO>(await context.Rangos.FirstOrDefaultAsync(rango => rango.Id == RangoId));
    }

    public static async Task<Results<NotFound, Ok>> PutRangoAsync(
    RangoDbContext context,
    IMapper mapper,
    int rangoId,
    [FromBody] RangoUpdateDTO rangoUpdateDTO)
    {
        var rangoEntity = await context
                        .Rangos
                        .FirstOrDefaultAsync(rango => rango.Id == rangoId);

        if (rangoEntity == null)
            return TypedResults.NotFound();

        mapper.Map(rangoUpdateDTO, rangoEntity);
        await context.SaveChangesAsync();
        return TypedResults.Ok();
    }

    public static async Task<Results<NotFound, NoContent>> DeleteRangoAsync(
    RangoDbContext context,
    int rangoId)
    {
        var rangoEntity = await context
                        .Rangos
                        .FirstOrDefaultAsync(rango => rango.Id == rangoId);

        if (rangoEntity == null)
            return TypedResults.NotFound();

        context.Rangos.Remove(rangoEntity);
        await context.SaveChangesAsync();

        return TypedResults.NoContent();
    }

}
