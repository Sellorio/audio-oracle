using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Sellorio.AudioOracle.Library.Results;

namespace Sellorio.AudioOracle.Web.Client.Library.Providers.DisableState;

public interface IDialogProvider
{
    Task<ValueResult<TResult>> ShowDialog<TResult>(Expression<Func<AoDialogBase<TResult>>> expression);
}
