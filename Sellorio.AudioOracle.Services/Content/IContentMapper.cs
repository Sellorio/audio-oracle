using Sellorio.AudioOracle.Data.Content;
using Sellorio.AudioOracle.Library.Mapping;
using Sellorio.AudioOracle.Models.Content;

namespace Sellorio.AudioOracle.Services.Content;

internal interface IContentMapper : IMap<FileInfoData, FileInfo>
{
}