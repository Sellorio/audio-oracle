using System.Threading;

namespace Sellorio.AudioOracle.Services.TaskQueue;

public class TaskHandlerContext
{
    public required int? ObjectId { get; init; }
    public required int? ObjectId2 { get; init; }
    public required CancellationToken CancellationToken { get; init; }
}
