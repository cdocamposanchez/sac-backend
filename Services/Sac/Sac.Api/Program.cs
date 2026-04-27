#region

using BuildingBlocks.Source.Infrastructure.Config;
using Microsoft.AspNetCore.HttpOverrides;
using Sac.Api;
using Sac.Application;
using Sac.Infrastructure;
using Sac.Infrastructure.Bootstrap;

#endregion

var builder = WebApplication.CreateBuilder(args);

EnvironmentHelper.LoadEnv();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration)
    .AddApiServices(builder.Configuration);

builder.WebHost.ConfigureKestrel(options =>
{
    options.AddServerHeader = false;
    var port = int.Parse(Environment.GetEnvironmentVariable("HTTP_PORT") ?? "8080");
    options.ListenAnyIP(port);
});

var app = builder.Build();
app.UseForwardedHeaders();
app.UseApiServices();

// Bootstrap al arranque:
//  1) Crea el esquema (migraciones o EnsureCreated si no hay migraciones registradas).
//  2) Crea el primer Director con BOOTSTRAP_DIRECTOR_* si no existe ninguno.
// Idempotente: si ya está todo listo, no hace nada.
await DatabaseBootstrapper.RunAsync(app.Services);

await app.RunAsync();
