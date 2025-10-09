using Sellorio.AudioOracle.Library.Results;
using System.IO;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.ServiceInterfaces.Content;

public interface IDataFileService
{
    Task<Result> PostDataFileAsync(string fileName, long fileSize, Stream contents);
}
