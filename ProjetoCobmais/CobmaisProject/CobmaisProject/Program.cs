//using Serilog;
//using Serilog.Formatting.Compact;
using Microsoft.OpenApi.Models;
using System.Reflection;
using CobmaisProject.Domain.Interfaces;
using CobmaisProject.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using CobmaisProject.Application.Service;
using CobmaisProject.Infrastructure.ExternalServices;

#region Serilog //Para o caso da utilização de Serilog
//var logFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
//Directory.CreateDirectory(logFolder);

//Log.Logger = new LoggerConfiguration()
//    .MinimumLevel.Information()
//    .WriteTo.File(new CompactJsonFormatter(),
//          Path.Combine(logFolder, ".json"),
//          retainedFileCountLimit: 3,
//          rollingInterval: RollingInterval.Day)
//    .WriteTo.File(Path.Combine(logFolder, ".log"),
//          retainedFileCountLimit: 3,
//          rollingInterval: RollingInterval.Day)
//    .CreateLogger();
#endregion

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Habilitar o uso do serilog.
//builder.Host.UseSerilog();

#region Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Projeto de Candidatura - Cobmais",
        Version = "v1",
        Description = @"
**Descrição do Projeto:**  

Este projeto foi desenvolvido para gerenciar a importação, atualização e exportação de dívidas utilizando uma API externa. As funcionalidades incluem:  

Importação de dados de contratos a partir de arquivos CSV:
- Manualmente, enviando o arquivo CSV;
- Automaticamente, buscando o arquivo CSV no diretório especificado;

Atualização de dívidas utilizando a API Cobmais.

Exportação de dados atualizados para arquivos CSV.

Manutenção de logs de atualizações no banco de dados.

A estrutura utilizada no projeto foi a DDD (Domain Driven Design).

**Autor:** Vinicius Dias Sant'Ana  
**Contato:** [vinicius28santana@gmail.com](mailto:vinicius28santana@gmail.com)  
**Repositório:** [GitHub](https://github.com/Vin-icius/CobmaisProject)  
",
        Contact = new OpenApiContact
        {
            Name = "Suporte",
            Email = "vinicius28santana@gmail.com",
            Url = new Uri("https://github.com/Vin-icius/CobmaisProject"),
        },
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
});
#endregion

#region IOC

builder.Services.AddScoped<IContratoRepository, ContratoRepository>();
builder.Services.AddScoped<IContratoService, ContratoService>();
builder.Services.AddScoped<ContratoService>();
builder.Services.AddScoped<ContratoRepository>();

builder.Services.AddSingleton<SqlConnection>(sp =>
{
    var connectionString = Environment.GetEnvironmentVariable("stringConexao");
    return new SqlConnection(connectionString);
});

builder.Services.AddHttpClient<CobmaisApiService>(client =>
{
    client.BaseAddress = new Uri("http://api.cobmais.com.br/testedev/calculo");
});

builder.Services.AddScoped<ILogAtualizacaoDividaRepository, LogAtualizacaoDividaRepository>();
builder.Services.AddScoped<CobmaisApiService>();

#endregion

#region Lendo o appsettings
var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

string pathAppsettings = "appsettings.json";

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile(pathAppsettings)
    .Build();

Environment.SetEnvironmentVariable("stringConexao", config.GetConnectionString("DefaultConnection"));
#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    c.RoutePrefix = ""; //habilitar a página inicial da API ser a doc.
    c.DocumentTitle = "Gerenciamento de Produtos - API V1";
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();