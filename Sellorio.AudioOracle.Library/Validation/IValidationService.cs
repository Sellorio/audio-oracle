using System;
using System.Threading.Tasks;
using Sellorio.AudioOracle.Library.Results;

namespace Sellorio.AudioOracle.Library.Validation;

public interface IValidationService
{
    Result<TObject> Validate<TObject>(TObject obj, Action<IValidationBuilder<TObject>> validate);
    Task<Result<TObject>> ValidateAsync<TObject>(TObject obj, Func<IValidationBuilder<TObject>, Task> validate);
}
