using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MudBlazor;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Library.Results.Messages;

namespace Sellorio.AudioOracle.Web.Client.Library.Providers.DisableState;

internal class DialogProvider(DisableStateScope disableStateScope, IDialogService dialogService) : IDialogProvider
{
    public async Task<ValueResult<TResult>> ShowDialog<TResult>(Expression<Func<AoDialogBase<TResult>>> expression)
    {
        if (disableStateScope.IsDisabled)
        {
            return ResultMessage.Error("Cannot create a dialog since the current disable scope is disabled.");
        }

        var memberInitExpression = (MemberInitExpression)expression.Body;
        var bindings = memberInitExpression.Bindings;
        var dialogParameters = new DialogParameters();

        foreach (var binding in bindings.Cast<MemberAssignment>())
        {
            var value = Expression.Lambda(binding.Expression).Compile().DynamicInvoke();
            dialogParameters.Add(binding.Member.Name, value);
        }

        DialogResult result;

        try
        {
            disableStateScope.UpdateState(true);
            var dialog = await dialogService.ShowAsync(memberInitExpression.NewExpression.Constructor!.DeclaringType!, null, dialogParameters);
            result = (await dialog.Result)!;
        }
        finally
        {
            disableStateScope.UpdateState(false);
        }

        if (result.Canceled)
        {
            return ResultMessage.Error("Action was cancelled.");
        }

        return (TResult)result.Data!;
    }
}
