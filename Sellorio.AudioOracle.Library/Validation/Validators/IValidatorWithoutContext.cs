using System;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Library.Validation.Validators;

[Obsolete("Do not inherit directly from this type. Use IValidator instead.")]
public interface IValidatorWithoutContext<TObject>
{
    Task ValidateAsync(IValidationBuilder<TObject> validate);
}
