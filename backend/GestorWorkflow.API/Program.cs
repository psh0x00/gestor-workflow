using GestorWorkflow.Data.Context;
using Microsoft.EntityFrameworkCore;
using GestorWorkflow.Core.Interfaces;
using GestorWorkflow.Core.Services;
using Microsoft.OpenApi.Models;
using System.Reflection;
using GestorWorkflow.Data.Repositories;
using GestorWorkflow.Data.Mappers;
using GestorWorkflow.Core.Entities;
using GestorWorkflow.Data.Models;

var builder = WebApplication.CreateBuilder(args);

// Connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Registar o DbContext
builder.Services.AddDbContext<GestorWorkflowDbContext>(options =>
    options.UseSqlServer(connectionString));

// Adiciona controladores, serviços, etc.
builder.Services.AddControllers();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Gestor Workflow API",
        Version = "v1",
        Description = "API para gestão de workflows",
        Contact = new OpenApiContact
        {
            Name = "Suporte",
            Email = "suporte@gestorworkflow.com"
        }
    });

    // Set the comments path for the Swagger JSON and UI
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

// Register mappers
builder.Services.AddScoped<IMapper<EstadoEntity, EstadoModelo>, EstadoMapper>();
builder.Services.AddScoped<IMapper<TransicaoEntity, TransicaoModelo>, TransicaoMapper>();
builder.Services.AddScoped<IMapper<WorkflowModeloEntity, WorkflowModelo>, WorkflowModeloMapper>();
builder.Services.AddScoped<IMapper<WorkflowInstanciaEntity, WorkflowInstancia>, WorkflowInstanciaMapper>();
builder.Services.AddScoped<IMapper<UtilizadorEntity, Utilizador>, UtilizadorMapper>();
builder.Services.AddScoped<IMapper<PermissaoEntity, Permissao>, PermissaoMapper>();
builder.Services.AddScoped<IMapper<PreCondicaoEntity, PreCondicao>, PreCondicaoMapper>();
builder.Services.AddScoped<IMapper<PosCondicaoEntity, PosCondicao>, PosCondicaoMapper>();
builder.Services.AddScoped<IMapper<RegistoTransicaoEntity, TransicaoInstancia>, RegistoTransicaoMapper>();

// Register UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register services
builder.Services.AddScoped<IWorkflowModeloService, WorkflowModeloService>();
builder.Services.AddScoped<IEstadoService, EstadoService>();
builder.Services.AddScoped<ITransicaoService, TransicaoService>();
builder.Services.AddScoped<IPermissaoService, PermissaoService>();
builder.Services.AddScoped<IWorkflowInstanciaService, WorkflowInstanciaService>();

var app = builder.Build();

// Configuração de pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gestor Workflow API V1");
        c.RoutePrefix = string.Empty; // To serve the Swagger UI at the app's root
    });
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.Run();