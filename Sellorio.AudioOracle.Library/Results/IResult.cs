using System.Collections.Generic;
using System.Linq;
using Sellorio.AudioOracle.Library.Results.Messages;

namespace Sellorio.AudioOracle.Library.Results;

public interface IResult
{
    bool WasSuccess { get; }
    bool WasFailure => !WasSuccess;
    IReadOnlyList<ResultMessage> Messages { get; }

    string ToDisplay()
    {
        return string.Join("\r\n", Messages.OrderBy(x => (int)x.Severity).Select(x => x.ToDisplay()));
    }
}
