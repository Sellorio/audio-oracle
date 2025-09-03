using System.Threading.Tasks;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Models.Content;

namespace Sellorio.AudioOracle.Services.Content;
internal interface IFileService
{
    Task<ValueResult<FileInfo>> CreateFileFromUrlAsync(string url, params FileType[] acceptedTypes);
}