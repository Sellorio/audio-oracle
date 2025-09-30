using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Sellorio.AudioOracle.Library.Results;

namespace Sellorio.AudioOracle.Web.Client.Library.Providers.DisableState;

public interface IDialogProvider
{
    Task<ValueResult<TResult>> ShowDialogAsync<TResult>(Expression<Func<AoDialogBase<TResult>>> expression);
    Task<Result> ShowDialogAsync(Expression<Func<AoDialogBase>> expression);
}
