using System.Threading.Tasks;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Models.Content;

namespace Sellorio.AudioOracle.Services.Content;

public interface IFileService
{
    Task<ValueResult<FileInfo>> CreateFileAsync(FileType type, System.IO.Stream stream);
    Task<ValueResult<FileInfo>> CreateFileFromUrlAsync(string url, params FileType[] acceptedTypes);
    Task<Result> DeleteAsync(int fileInfoId);
    Task<ValueResult<FileInfo>> GetByUrlIdAsync(string urlId);
    Task<ValueResult<StreamWithFilename>> GetTrackMediaStreamAsync(int trackId);
}