using System.ComponentModel.DataAnnotations;

namespace Sellorio.AudioOracle.Models.Metadata;

public enum ArtistGender
{
    [Display(Name = "")]
    Unspecified,
    [Display(Name = "Male")]
    Male,
    [Display(Name = "Female")]
    Female,
    [Display(Name = "Other")]
    Other
}
