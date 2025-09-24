using Sellorio.AudioOracle.Data.Content;
using Sellorio.AudioOracle.Library.Mapping;
using Sellorio.AudioOracle.Models.Content;

namespace Sellorio.AudioOracle.Services.Content;

internal class ContentMapper : StaticMapperBase<ContentMapper>, IContentMapper
{
    public FileInfo Map(FileInfoData from) => Map<FileInfo>(from);
    public FileContent Map(FileContentData from) => Map<FileContent>(from);
}
