using System;
using Microsoft.EntityFrameworkCore;

namespace Sellorio.AudioOracle.Data.Sessions;

[Index(nameof(Guid), IsUnique = true)]
public class SessionData
{
    public int Id { get; set; }

    public required Guid Guid { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public required DateTimeOffset LastAccessedAt { get; set; }
}
