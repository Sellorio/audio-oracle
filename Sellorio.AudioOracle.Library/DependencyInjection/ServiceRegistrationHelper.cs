using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Sellorio.AudioOracle.Library.DependencyInjection;

public static class ServiceRegistrationHelper
{
    public static void EnsureAllServicesAreRegistered(
        IServiceCollection services,
        IEnumerable<Assembly> assembliesToCheck,
        IEnumerable<string>? namespacesToInclude = null,
        IEnumerable<Type>? typesToExclude = null)
    {
#if DEBUG
        var registeredTypes = services.Select(x => x.ServiceType).ToHashSet();
        var notRegisteredServices =
            assembliesToCheck
                .SelectMany(x => x.GetTypes())
                .Where(x =>
                    x.IsInterface &&
                    !registeredTypes.Contains(x) &&
                    (typesToExclude == null || !typesToExclude.Contains(x)) &&
                    (namespacesToInclude == null || namespacesToInclude.Any(y => x.Namespace != null && (x.Namespace == y || x.Namespace.StartsWith(y + '.')))))
                .ToArray();

        if (notRegisteredServices.Length != 0)
        {
            throw new ApplicationException("The following services have not been registered: " + string.Join(", ", notRegisteredServices.Select(x => x.Name)) + ".");
        }
#endif
    }
}
