using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;
using RangoAgil.API.EndPoitHandlers;
using RangoAgil.API.Entities;
using RangoAgil.API.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RangoDbContext>(
    o => o.UseSqlite(builder.Configuration["ConnectionStrings:RangoDbConnStr"]));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

app.MapGet("/", () => "Bem vindo ao Rango's");

#region COMENTADOS
//app.MapGet("/rangos/{id:int}", async (RangoDbContext context, int id) =>
//{
//    return await context.Rangos.FirstOrDefaultAsync(rango => rango.Id == id);
//});

//app.MapGet("/rangosFromQuery", async (RangoDbContext context, [FromQuery(Name = "RangoId")] int id) =>
//{
//    return await context.Rangos.FirstOrDefaultAsync(rango => rango.Id == id);
//});

//app.MapGet("/rango/{nome}", async (RangoDbContext context, string nome) =>
//{
//    return await context.Rangos.FirstOrDefaultAsync(rango => rango.Nome == nome);
//});

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

#endregion

var rangosEndpoints = app.MapGroup("/rangos");
var rangosComIdEndpoints = rangosEndpoints.MapGroup("/{rangoId:int}");
var rangosComIngredientesEndpoints = rangosComIdEndpoints.MapGroup("/ingredientes");

//using automapper

rangosEndpoints.MapGet("", RangosHandlers.GetRangosAsync);

rangosEndpoints.MapPost("", RangosHandlers.CreateRangoAsync);

rangosComIdEndpoints.MapGet("", RangosHandlers.GetRangoById)
                    .WithName("GetRangoById");

rangosComIdEndpoints.MapPut("", RangosHandlers.UpdateRangoAsync);

rangosComIdEndpoints.MapDelete("", RangosHandlers.DeleteRangoAsync);

rangosComIngredientesEndpoints.MapGet("", IngredientesHandlers.GetIngredientesByRangoIdAsync);


app.Run();
