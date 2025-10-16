using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Library.Results.Messages;
using Sellorio.AudioOracle.Providers;

namespace Sellorio.AudioOracle.Services;

internal class ProviderInvocationService(IServiceProvider serviceProvider, ILogger<ProviderInvocationService> logger) : IProviderInvocationService
{
    public async Task<ValueResult<IList<TOutput>>> InvokeAsync<TProvider, TOutput>(Func<TProvider, Task<ValueResult<TOutput>>> providerInvocation)
        where TProvider : IProvider
    {
        var providersEnumerable = serviceProvider.GetRequiredService<IEnumerable<TProvider>>();
        var providers = providersEnumerable.ToArray();

        return await InvokeAsync(providers, providerInvocation);
    }

    public async Task<ValueResult<IList<TOutput>>> InvokeAsync<TProvider, TOutput>(IList<string>? providerNames, Func<TProvider, Task<ValueResult<TOutput>>> providerInvocation)
        where TProvider : IProvider
    {
        var providersEnumerable = serviceProvider.GetRequiredService<IEnumerable<TProvider>>();
        var providers = providersEnumerable.Where(x => providerNames == null || providerNames.Contains(x.ProviderName)).ToArray();

        return await InvokeAsync(providers, providerInvocation);
    }

    public async Task<ValueResult<TOutput>> InvokeAsync<TProvider, TOutput>(string providerName, Func<TProvider, Task<ValueResult<TOutput>>> providerInvocation)
        where TProvider : IProvider
    {
        var providersEnumerable = serviceProvider.GetRequiredService<IEnumerable<TProvider>>();
        var provider = providersEnumerable.FirstOrDefault(x => x.ProviderName == providerName);

        if (provider == null)
        {
            return ResultMessage.Error($"Missing expected {typeof(TProvider).Name} implementation for {providerName}.");
        }

        try
        {
            return await providerInvocation.Invoke(provider);
        }
        catch (Exception ex)
        {
            var messages = new List<ResultMessage>();
            HandleException(ex, provider.ProviderName, messages, ResultMessageSeverity.Critical);
            return messages[0];
        }
    }

    public async Task<Result> InvokeAsync<TProvider>(string providerName, Func<TProvider, Task<Result>> providerInvocation)
        where TProvider : IProvider
    {
        var providersEnumerable = serviceProvider.GetRequiredService<IEnumerable<TProvider>>();
        var provider = providersEnumerable.FirstOrDefault(x => x.ProviderName == providerName);

        if (provider == null)
        {
            return ResultMessage.Error($"Missing expected {typeof(TProvider).Name} implementation for {providerName}.");
        }

        try
        {
            return await providerInvocation.Invoke(provider);
        }
        catch (Exception ex)
        {
            var messages = new List<ResultMessage>();
            HandleException(ex, provider.ProviderName, messages, ResultMessageSeverity.Critical);
            return messages[0];
        }
    }

    private async Task<ValueResult<IList<TOutput>>> InvokeAsync<TProvider, TOutput>(TProvider[] providers, Func<TProvider, Task<ValueResult<TOutput>>> providerInvocation)
        where TProvider : IProvider
    {
        var tasks = new Task<ValueResult<TOutput>>[providers.Length];
        var results = new List<TOutput>();
        var messages = new List<ResultMessage>();

        for (int i = 0; i < providers.Length; i++)
        {
            var provider = providers[i];

            try
            {
                tasks[i] = providerInvocation.Invoke(provider);
            }
            catch (Exception ex)
            {
                HandleException(ex, provider.ProviderName, messages, ResultMessageSeverity.Warning);
            }
        }

        for (int i = 0; i < tasks.Length; i++)
        {
            var task = tasks[i];
            var provider = providers[i];

            if (task == null)
            {
                continue;
            }

            try
            {
                var result = await task;

                if (result.WasSuccess)
                {
                    results.Add(result.Value!);
                    messages.AddRange(result.Messages);
                }
                else
                {
                    foreach (var message in result.Messages)
                    {
                        if (message.Severity is ResultMessageSeverity.Information or ResultMessageSeverity.Warning)
                        {
                            messages.Add(message);
                        }
                        else
                        {
                            messages.Add(ResultMessage.Warning($"Failure when invoking {provider.ProviderName} provider.\r\n{message.Text}"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, provider.ProviderName, messages, ResultMessageSeverity.Warning);
            }
        }

        return ValueResult<IList<TOutput>>.Success(results, messages);
    }

    private void HandleException(Exception ex, string providerName, List<ResultMessage> messages, ResultMessageSeverity severity)
    {
        var messageText = $"Failure when invoking {providerName} provider. See logs for more info.";

        switch (severity)
        {
            case ResultMessageSeverity.Error:
                messages.Add(ResultMessage.Error(messageText));
                break;
            case ResultMessageSeverity.Warning:
                messages.Add(ResultMessage.Warning(messageText));
                break;
            case ResultMessageSeverity.Information:
                messages.Add(ResultMessage.Information(messageText));
                break;
            case ResultMessageSeverity.Critical:
                messages.Add(ResultMessage.Critical(messageText));
                break;
            default:
                throw new NotSupportedException("Unsupported severity used in HandleException.");
        }

        logger.LogException(ex, "Failure when invoking {provider.SourceName} provider.", providerName);
    }
}
