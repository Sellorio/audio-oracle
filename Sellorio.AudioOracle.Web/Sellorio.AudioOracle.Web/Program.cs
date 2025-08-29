using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sellorio.AudioOracle.Data;
using Sellorio.AudioOracle.Models;
using Sellorio.AudioOracle.Services;
using Sellorio.AudioOracle.Web.Components;
using Sellorio.AudioOracle.Web.Framework;

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(EnvironmentVariables.AdminPassword)))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.Error.WriteLine($"Admin password ({EnvironmentVariables.AdminPassword}) environment variable has not been set. This is required.");
    Console.ForegroundColor = ConsoleColor.White;
    return;
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services
    .AddScoped<AuthorizeFilter>()
    .AddDbContext<DatabaseContext>()
    .AddAudioOracleServerSideServices()
    ;

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

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
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
