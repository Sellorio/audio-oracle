using Microsoft.Extensions.DependencyInjection;
using Sellorio.AudioOracle.Web.Client.Library.Services;

namespace Sellorio.AudioOracle.Web.Client.Library;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAoServices(this IServiceCollection services)
    {
        return services.AddScoped<IResultPopupService, ResultPopupService>();
    }
}
