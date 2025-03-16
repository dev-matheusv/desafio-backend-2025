using OxsBank.Application.Interfaces;
using OxsBank.Infrastructure.Configurations;
using OxsBank.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Adiciona os serviços do banco de dados
builder.Services.AddDatabaseConfiguration(builder.Configuration);
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IReceitaWsService, ReceitaWsService>();
builder.Services.AddHttpClient();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();