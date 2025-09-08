using System;

namespace Sellorio.AudioOracle.Models.TaskQueue;

public class QueuedTask
{
    public int Id { get; init; }
    public required string HandlerName { get; set; }
    public required int? ObjectId { get; set; }
    public required int? ObjectId2 { get; set; }
    public DateTime? QueuedAt { get; set; }
    public byte Lives { get; set; }
}
