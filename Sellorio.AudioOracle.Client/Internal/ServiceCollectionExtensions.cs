using System;
using System.Linq;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Sellorio.AudioOracle.Client.Sessions;

namespace Sellorio.AudioOracle.Client.Internal;

internal static class ServiceCollectionExtensions
{
    public static IHttpClientBuilder TryAddRestClient<TInterface, TImplementation>(this IServiceCollection services)
        where TImplementation : TInterface
        where TInterface : class
    {
        return TryAddRestClient<TInterface, TImplementation>(services, (Action<HttpClient>)null);
    }

    public static IHttpClientBuilder TryAddRestClient<TInterface, TImplementation>(this IServiceCollection services, Action<HttpClient> configureClient)
        where TImplementation : TInterface
        where TInterface : class
    {
        if (services.Any(x => x.ServiceType == typeof(TInterface)))
        {
            return null;
        }

        var httpClientName = ">>" + typeof(TImplementation).Name;

        var httpClientBuilder =
            configureClient == null
                ? services.AddHttpClient(httpClientName)
                : services.AddHttpClient(httpClientName, configureClient);

        TryAddRestClient<TInterface, TImplementation>(services, httpClientName);

        return httpClientBuilder;
    }

    public static IServiceCollection TryAddRestClient<TInterface, TImplementation>(this IServiceCollection services, string httpClientName)
        where TImplementation : TInterface
        where TInterface : class
    {
        if (services.Any(x => x.ServiceType == typeof(TInterface)))
        {
            return services;
        }

        var constructors = typeof(TImplementation).GetConstructors();

        if (constructors.Length != 1)
        {
            throw new InvalidOperationException("Only a single constructor is supported.");
        }

        var constructor = constructors[0];

        services.AddScoped<TInterface>(svc =>
        {
            var httpClientFactory = svc.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient(httpClientName);
            var sessionTokenProvider = svc.GetRequiredService<IAudioOracleSessionTokenProvider>();
            var restClient = new RestClient(httpClient, sessionTokenProvider);

            var constructorParameters =
                constructor.GetParameters()
                    .Select(x => x.ParameterType == typeof(IRestClient) ? restClient : svc.GetRequiredService(x.ParameterType))
                    .ToArray();

            return (TImplementation)constructor.Invoke(constructorParameters);
        });

        return services;
    }
}
