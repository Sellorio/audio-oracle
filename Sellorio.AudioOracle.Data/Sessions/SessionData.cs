using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Sellorio.AudioOracle.Data.Sessions;

[Index(nameof(Guid), IsUnique = true)]
public class SessionData
{
    public int Id { get; set; }

    [Required]
    public Guid? Guid { get; set; }

    [Required]
    public DateTimeOffset? CreatedAt { get; set; }

    [Required]
    public DateTimeOffset? LastAccessedAt { get; set; }
}
