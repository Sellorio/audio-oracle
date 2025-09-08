using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Sellorio.AudioOracle.Data.TaskQueue;

[Index(nameof(QueuedAt))]
public class QueuedTaskData
{
    public int Id { get; init; }

    [Required, StringLength(100)]
    public required string HandlerName { get; set; }

    public required int? ObjectId { get; set; }

    public required int? ObjectId2 { get; set; }

    // Supports future dated tasks for rescheduling of a failed operation
    // Null value represents frozen/suspended tasks - tasks that need to be manually re-queued due to failing multiple times
    public DateTime? QueuedAt { get; set; } = DateTime.UtcNow;

    // When lives reaches 0, the task is suspended
    public byte Lives { get; set; } = 5;
}
