using Microsoft.EntityFrameworkCore;
using Sellorio.AudioOracle.Data;
using Sellorio.AudioOracle.Services.Content;
using Sellorio.AudioOracle.Services.Metadata;
using System.IO;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Services.TaskQueue.Handlers;

internal class RefreshTrackTags(DatabaseContext databaseContext, IFileTagsService fileTagsService, IMetadataMapper metadataMapper) : ITaskHandler
{
    public async Task HandleAsync(TaskHandlerContext context)
    {
        var trackData =
            await databaseContext.Tracks
                .Include(x => x.Artists)!.ThenInclude(x => x.Artist)
                .Include(x => x.Album).ThenInclude(x => x!.AlbumArt)
                .Include(x => x.Album).ThenInclude(x => x!.Artists)!.ThenInclude(x => x.Artist)
                .SingleOrDefaultAsync(x => x.Id == context.ObjectId!.Value);

        if (trackData == null || trackData.Filename == null || !File.Exists(trackData.Filename))
        {
            return;
        }

        var track = metadataMapper.Map(trackData);
        var album = metadataMapper.Map(trackData.Album!);

        await fileTagsService.UpdateFileTagsAsync(trackData.Filename, album, track);
    }
}
