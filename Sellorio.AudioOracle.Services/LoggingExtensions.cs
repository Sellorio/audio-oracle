using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Sellorio.AudioOracle.Services;
internal static class LoggingExtensions
{
    [SuppressMessage("Usage", "CA2254:Template should be a static expression", Justification = "Helper function.")]
    public static void LogException<TService>(this ILogger<TService> logger, Exception exception, string messageTemplate, params object[] messageArgs)
    {
        logger.LogError(
            messageTemplate + "\r\n{ExceptionType}:\r\n{ExceptionMessage}",
            [..messageArgs, exception.GetType().AssemblyQualifiedName, exception.Message]);
    }
}
