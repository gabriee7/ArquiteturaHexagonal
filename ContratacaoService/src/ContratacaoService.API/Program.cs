using Asp.Versioning;
using ContratacaoService.API.Middlewares;
using ContratacaoService.Application.Contratacoes.Interfaces;
using ContratacaoService.Application.Contratacoes.UseCases;
using ContratacaoService.Domain.Ports.External.PropostaService;
using ContratacaoService.Domain.Ports.Repositories;
using ContratacaoService.Infrastructure.Adapters.External;
using ContratacaoService.Infrastructure.Adapters.Messaging;
using ContratacaoService.Infrastructure.Adapters.Persistence;
using ContratacaoService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using ContratacaoService.Infrastructure.Adapters.Messaging.Extensions;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IPropostaGateway, PropostaGatewayAdapter>();
builder.Services.AddScoped<IContratacaoRepository, ContratacaoRepository>();

builder.Services.AddScoped<ICriarContratacaoUseCase, CriarContratacaoUseCase>();
builder.Services.AddScoped<IObterContratacaoPorIdUseCase, ObterContratacaoPorIdUseCase>();
builder.Services.AddScoped<IDeletarContratacaoUseCase, DeletarContratacaoUseCase>();
builder.Services.AddScoped<IObterContratacaoPorPropostaIdUseCase, ObterContratacaoPorPropostaIdUseCase>();
builder.Services.AddScoped<IBuscarTodasContratacoesUseCase, BuscarTodasContratacoesUseCase>();

builder.Services.AddControllers()
  .AddJsonOptions(options =>
  {
      options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
  });

var apiVersioningBuilder = builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

apiVersioningBuilder.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Contratacao API", Version = "v1" });
});

builder.Services.AddMassTransitWithAutoDiscovery(
    builder.Configuration,
    typeof(PropostaAprovadaConsumer).Assembly
);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        if (context.Database.GetPendingMigrations().Any())
            context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro ao rodar as migrations.");
    }
}

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Contratacao API v1");
    });
}

app.UseAuthorization();
app.MapControllers();

app.Run();