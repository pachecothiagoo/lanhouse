using LanHouse.Clientes;
using LanHouse.Clientes.Infra;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
    });
});

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);

GeradorDeServicos.Configuracao = builder.Configuration;
GeradorDeServicos.ServiceProvider = builder.Services.BuildServiceProvider();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
    // Cria o banco e as tabelas caso ainda não existam.
    dataContext.Database.EnsureCreated();
    DadosIniciais.Semear(dataContext);
}

var forwardedHeadersOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                       ForwardedHeaders.XForwardedProto
};

forwardedHeadersOptions.KnownIPNetworks.Clear();
forwardedHeadersOptions.KnownProxies.Clear();
app.UseForwardedHeaders(forwardedHeadersOptions);

app.MapOpenApi();
app.MapScalarApiReference();

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Raiz redireciona para a documentação interativa; /health serve para "acordar" e monitorar o serviço.
app.MapGet("/", () => Results.Redirect("/scalar"));
app.MapGet("/health", () => Results.Ok(new { servico = "Clientes", status = "ok" }));

app.Run();
