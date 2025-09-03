using Sellorio.AudioOracle.Data.Content;
using Sellorio.AudioOracle.Models.Content;

namespace Sellorio.AudioOracle.Services.Content;
internal interface IContentMapper
{
    FileInfo Map(FileInfoData from);
}