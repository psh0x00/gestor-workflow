using GestorWorkflow.Data.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Registar o DbContext
builder.Services.AddDbContext<GestorWorkflowDbContext>(options =>
    options.UseSqlServer(connectionString));

// Adiciona controladores, serviços, etc.
builder.Services.AddControllers();

var app = builder.Build();

// Configuração de pipeline
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.Run();