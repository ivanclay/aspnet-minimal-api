using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RangoAgil.API.DbContexts;
using RangoAgil.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RangoDbContext>(
    o => o.UseSqlite(builder.Configuration["ConnectionStrings:RangoDbConnStr"]));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddProblemDetails();
builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddAuthorization();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequireAdminFromBrazil", policy =>
        policy
          .RequireRole("admin")
          .RequireClaim("country", "brazil"));

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("TokenAuthRango",
        new()
        {
            Name = "Authorization",
            Description = "Token baseado em Autentica��o e Autoriza��o",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            In = ParameterLocation.Header
        }
    );
    options.AddSecurityRequirement(new()
            {
                {
                    new ()
                    {
                        Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                            Id = "TokenAuthRango"
                        }
                    },
                    new List<string>()
                }
            }
    );
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
    //app.UseExceptionHandler(configureApplicationBuilder =>
    //{
    //    configureApplicationBuilder.Run(
    //        async context =>
    //        {
    //            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
    //            context.Response.ContentType = "text/html";
    //            await context.Response.WriteAsync("An unexpected problem happened.");
    //        });
    //});
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "Bem vindo ao Rango's");

app.RegisterRangosEndpoints();
app.RegisterIngredientesEndpoints();

app.Run();
