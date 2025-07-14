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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

    // JWT Auth no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando o esquema Bearer. Exemplo: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
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

// Configuração de autenticação JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
    // Adiciona eventos para logar falhas de autenticação
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"[JWT] Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine($"[JWT] Token validado para: {context.Principal?.Identity?.Name ?? "(sem Name)"}");
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            Console.WriteLine($"[JWT] Challenge: {context.Error}, {context.ErrorDescription}");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
    );
});

var app = builder.Build();


// Swagger sempre disponível
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gestor Workflow API V1");
    c.RoutePrefix = string.Empty; // To serve the Swagger UI at the app's root
});

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors("AllowFrontend");

app.UseAuthentication(); // Adicionado antes do Authorization
app.UseAuthorization();

app.MapControllers();

app.Run();