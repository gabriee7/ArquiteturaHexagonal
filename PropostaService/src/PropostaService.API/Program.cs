using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PropostaService.API.Middlewares;
using PropostaService.Application.Propostas.Interfaces;
using PropostaService.Application.Propostas.UseCases;
using PropostaService.Domain.Ports.Messaging;
using PropostaService.Domain.Ports.Repositories;
using PropostaService.Infrastructure.Adapters.Messaging;
using PropostaService.Infrastructure.Adapters.Messaging.Config;
using PropostaService.Infrastructure.Adapters.Persistence;
using PropostaService.Infrastructure.Data;
using Serilog;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();
builder.Host.UseSerilog();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
  options.UseNpgsql(connectionString));

builder.Services.AddScoped<IPropostaRepository, PropostaRepository>();
builder.Services.AddScoped<ICriarPropostaUseCase, CriarPropostaUseCase>();
builder.Services.AddScoped<IBuscarPropostaPorIdUseCase, BuscarPropostaPorIdUseCase>();
builder.Services.AddScoped<IBuscarTodasPropostasUseCase, BuscarTodasPropostasUseCase>();
builder.Services.AddScoped<IAtualizarPropostaUseCase, AtualizarPropostaUseCase>();
builder.Services.AddScoped<IDeletarPropostaUseCase, DeletarPropostaUseCase>();
builder.Services.AddScoped<IAprovarPropostaUseCase, AprovarPropostaUseCase>();
builder.Services.AddScoped<IRejeitarPropostaUseCase, RejeitarPropostaUseCase>();

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
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Proposta API", Version = "v1" });
});

builder.Services.AddMassTransitProducer(builder.Configuration);
builder.Services.AddScoped<IMessageBus, MassTransitPublisherAdapter>();

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
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Proposta API v1");
    });
}

app.UseAuthorization();
app.MapControllers();

app.Run();