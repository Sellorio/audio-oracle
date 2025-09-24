using System;
using System.Net.Http;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MudBlazor.Services;
using Sellorio.AudioOracle.Client.Sessions;
using Sellorio.AudioOracle.Data;
using Sellorio.AudioOracle.Models;
using Sellorio.AudioOracle.Services;
using Sellorio.AudioOracle.Web.Client.Services;
using Sellorio.AudioOracle.Web.Components;
using Sellorio.AudioOracle.Web.Framework;
using Sellorio.AudioOracle.Web.TaskQueue;

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(EnvironmentVariables.AdminPassword)))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.Error.WriteLine($"Admin password ({EnvironmentVariables.AdminPassword}) environment variable has not been set. This is required.");
    Console.ForegroundColor = ConsoleColor.White;
    return;
}

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddHttpClient(nameof(Sellorio.AudioOracle.Web.Controllers.FileController))
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { ServerCertificateCustomValidationCallback = (_, _, _, _) => true });

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services
    .AddControllers().AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services
    .AddScoped<AuthorizeFilter>()
    .AddDbContext<DatabaseContext>()
    .AddAudioOracleServerSideServices()
    ;

builder.Services.AddHostedService<TaskQueueHostedService>();
builder.Services.AddScoped<AuthenticationStateProvider>();
builder.Services.AddScoped<Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider>(svc => svc.GetRequiredService<AuthenticationStateProvider>());
builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
builder.Services.AddScoped<IAudioOracleSessionTokenProvider, SessionTokenProvider>();
builder.Services.AddMudServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseAntiforgery();

app.MapStaticAssets();
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Sellorio.AudioOracle.Web.Client._Imports).Assembly);

using (var scope = app.Services.CreateScope())
{
    Console.WriteLine("Creating/updating database...");
    var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    await db.Database.MigrateAsync();
    Console.WriteLine("Finished creating/updating database.");
}

await app.RunAsync();
