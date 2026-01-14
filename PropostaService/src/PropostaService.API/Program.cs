using Microsoft.EntityFrameworkCore;
using PropostaService.API.Middlewares;
using PropostaService.Application.Propostas.Interfaces;
using PropostaService.Application.Propostas.UseCases;
using PropostaService.Domain.Ports.Repositories;
using PropostaService.Infrastructure.Adapters.Persistence;
using PropostaService.Infrastructure.Data;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<ICriarPropostaUseCase, CriarPropostaUseCase>();
builder.Services.AddScoped<IBuscarPropostaPorIdUseCase, BuscarPropostaPorIdUseCase>();
builder.Services.AddScoped<IBuscarTodasPropostasUseCase, BuscarTodasPropostasUseCase>();
builder.Services.AddScoped<IAtualizarPropostaUseCase, AtualizarPropostaUseCase>();
builder.Services.AddScoped<IDeletarPropostaUseCase, DeletarPropostaUseCase>();
builder.Services.AddScoped<IAprovarPropostaUseCase, AprovarPropostaUseCase>();
builder.Services.AddScoped<IRejeitarPropostaUseCase, RejeitarPropostaUseCase>();
builder.Services.AddScoped<IPropostaRepository, PropostaRepository>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();