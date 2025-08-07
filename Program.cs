using SalesIndicators.API.Data;
using Microsoft.EntityFrameworkCore;
using SalesIndicators.API.Services;
using Microsoft.Extensions.Options;
using SalesIndicators.API.Settings;

var builder = WebApplication.CreateBuilder(args);

// Adicionar serviços ao container
builder.Services.AddControllers();

// Configurar cache em memória
builder.Services.AddMemoryCache();

// Configurar a seção CacheSettings para injeção via IOptions<CacheSettings>
builder.Services.Configure<CacheSettings>(builder.Configuration.GetSection("CacheSettings"));

// Registrar o serviço SalesService para injeção de dependência
builder.Services.AddScoped<SalesService>();

// Configurar conexão com PostgreSQL via Entity Framework Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Adicionar suporte ao Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//csvExport
builder.Services.AddScoped<CsvExportService>();

var app = builder.Build();

// Middleware Swagger para ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Mapear rotas dos controllers
app.MapControllers();

app.Run();
