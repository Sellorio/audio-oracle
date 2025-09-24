using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using Sellorio.AudioOracle.Client;
using Sellorio.AudioOracle.Client.Sessions;
using Sellorio.AudioOracle.Web.Client.Services;

[assembly: InternalsVisibleTo("Sellorio.AudioOracle.Web.Client.UnitTests")]

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddSingleton<AuthenticationStateProvider>();
builder.Services.AddSingleton<Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider>(svc => svc.GetRequiredService<AuthenticationStateProvider>());

builder.Services.AddAudioOracleClientSideServices<SessionTokenProvider>(builder.HostEnvironment.BaseAddress);
builder.Services.AddMudServices();

builder.Services.AddSingleton<ILocalStorageService, LocalStorageService>();
builder.Services.AddSingleton<IAudioOracleSessionTokenProvider, SessionTokenProvider>();

await builder.Build().RunAsync();
