using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Providers;

namespace Sellorio.AudioOracle.Services;
internal interface IProviderInvocationService
{
    Task<ValueResult<IList<TOutput>>> InvokeAllAsync<TProvider, TOutput>(Func<TProvider, Task<ValueResult<TOutput>>> providerInvocation) where TProvider : IProvider;
    Task<ValueResult<TOutput>> InvokeAsync<TProvider, TOutput>(string providerName, Func<TProvider, Task<ValueResult<TOutput>>> providerInvocation) where TProvider : IProvider;
}