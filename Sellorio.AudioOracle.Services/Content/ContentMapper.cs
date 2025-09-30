using Sellorio.AudioOracle.Data.Content;
using Sellorio.AudioOracle.Library.Mapping;
using Sellorio.AudioOracle.Models.Content;

namespace Sellorio.AudioOracle.Services.Content;

internal class ContentMapper : MapperBase, IContentMapper
{
    public FileInfo Map(FileInfoData from) => Map<FileInfo>(from);
}
