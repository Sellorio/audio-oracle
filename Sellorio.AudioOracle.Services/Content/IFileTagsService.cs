using System.Threading.Tasks;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Models.Metadata;

namespace Sellorio.AudioOracle.Services.Content;

internal interface IFileTagsService
{
    Task<Result> UpdateFileTagsAsync(string filename, Album album, Track track);
}