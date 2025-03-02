using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;
using RangoAgil.API.Entities;
using RangoAgil.API.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RangoDbContext>(
    o => o.UseSqlite(builder.Configuration["ConnectionStrings:RangoDbConnStr"]));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

app.MapGet("/", () => "Bem vindo ao Rango's");

app.MapGet("/rango/{id:int}", async (RangoDbContext context, int id) =>
{
    return await context.Rangos.FirstOrDefaultAsync(rango => rango.Id == id);
});

app.MapGet("/rangoFromQuery", async (RangoDbContext context, [FromQuery(Name = "RangoId")] int id) =>
{
    return await context.Rangos.FirstOrDefaultAsync(rango => rango.Id == id);
});

app.MapGet("/rango/{nome}", async (RangoDbContext context, string nome) =>
{
    return await context.Rangos.FirstOrDefaultAsync(rango => rango.Nome == nome);
});



//using automapper

app.MapGet("/rangos", async Task<Results<NoContent, Ok<IEnumerable<RangoDTO>>>>
    (RangoDbContext context,
    IMapper mapper,
    [FromQuery(Name = "name")] string? rangoName) =>
{
    var rangoEntity = await context
                        .Rangos
                        .Where(rango => rangoName == null || rango.Nome.ToLower().Contains(rangoName.ToLower()))
                        .ToListAsync();
    if (rangoEntity == null || rangoEntity.Count <= 0)
        return TypedResults.NoContent();
    return TypedResults.Ok(mapper.Map<IEnumerable<RangoDTO>> (rangoEntity));
});

app.MapGet("/rango", async (
    RangoDbContext context,
    IMapper mapper,
    [FromHeader(Name = "RangoId")] int id) =>
{
    return mapper.Map<RangoDTO>(await context.Rangos.FirstOrDefaultAsync(rango => rango.Id == id));
})
    .WithName("GetRangoById");

app.MapGet("/rango/{rangoId:int}/ingredientes", async 
    (RangoDbContext context,
     IMapper mapper,
    int rangoId) =>
{
    return  mapper.Map<IEnumerable<IngredienteDTO>>((await context
                    .Rangos
                    .Include(rango => rango.Ingredientes)
                    .FirstOrDefaultAsync(rango => rango.Id == rangoId))?.Ingredientes);
});

//app.MapPost("/rango", async (
//    RangoDbContext context,
//    IMapper mapper,
//    [FromBody] RangoCreateDTO rangoCreateDTO,
//    LinkGenerator linkGenerator,
//    HttpContext httpContext) =>
//{
//    var rangoEntity = mapper.Map<Rango>(rangoCreateDTO);
//    context.Rangos.Add(rangoEntity);
//    await context.SaveChangesAsync();

//    var rangoReturn = mapper.Map<RangoDTO>(rangoEntity);

//    var linkToCreatedReturn = linkGenerator.GetUriByName(
//        httpContext,
//        "GetRangoById",
//        new { id = rangoReturn.Id }
//        );

//    return TypedResults.Created(
//        linkToCreatedReturn,
//        rangoReturn);
//});

app.MapPost("/rango", async Task<CreatedAtRoute<RangoDTO>> (
    RangoDbContext context,
    IMapper mapper,
    [FromBody] RangoCreateDTO rangoCreateDTO) =>
{
    var rangoEntity = mapper.Map<Rango>(rangoCreateDTO);
    context.Rangos.Add(rangoEntity);
    await context.SaveChangesAsync();

    var rangoReturn = mapper.Map<RangoDTO>(rangoEntity);

    return TypedResults.CreatedAtRoute(
                            rangoReturn,
                            "GetRangoById",
                            new { id = rangoReturn.Id });
});

app.MapPut("/rango/{id:int}", async Task<Results<NotFound, Ok>>  (
    RangoDbContext context, 
    IMapper mapper,
    int id,
    [FromBody] RangoUpdateDTO rangoUpdateDTO) =>
{
    var rangoEntity = await context
                    .Rangos
                    .FirstOrDefaultAsync(rango => rango.Id == id);
    
    if (rangoEntity == null)
        return TypedResults.NotFound();
    //return TypedResults.NoContent();

    mapper.Map(rangoUpdateDTO, rangoEntity);
    await context.SaveChangesAsync();
    return TypedResults.Ok();
});

app.MapDelete("/rango/{id:int}", async Task<Results<NotFound, NoContent>> (
    RangoDbContext context,
    int id) =>
{
    var rangoEntity = await context
                    .Rangos
                    .FirstOrDefaultAsync(rango => rango.Id == id);

    if (rangoEntity == null)
        return TypedResults.NotFound();

    context.Rangos.Remove(rangoEntity);
    await context.SaveChangesAsync();

    return TypedResults.NoContent();
});

app.Run();
