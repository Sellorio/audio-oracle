using System;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sellorio.AudioOracle.Data;
using Sellorio.AudioOracle.Data.Content;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Library.Results.Messages;
using Sellorio.AudioOracle.Models.Content;

namespace Sellorio.AudioOracle.Services.Content;

internal class FileService(IHttpClientFactory httpClientFactory, DatabaseContext databaseContext, IContentMapper contentMapper) : IFileService
{
    public async Task<ValueResult<FileInfo>> CreateFileFromUrlAsync(string url, params FileType[] acceptedTypes)
    {
        var httpClient = httpClientFactory.CreateClient("NoSSL");
        var responseMessage = await httpClient.GetAsync(url);

        if (!responseMessage.IsSuccessStatusCode)
        {
            return ResultMessage.Error("Failed to download file.");
        }

        var contentType = responseMessage.Content.Headers.ContentType!.MediaType;
        FileType? fileType = contentType switch
        {
            MediaTypeNames.Image.Jpeg => FileType.ImageJpeg,
            MediaTypeNames.Image.Png => FileType.ImagePng,
            _ => null
        };

        if (fileType == null)
        {
            return ResultMessage.Error("Unsupported file type.");
        }

        var data = await responseMessage.Content.ReadAsByteArrayAsync();

        var fileContentData = new FileContentData
        {
            Data = data
        };

        var fileInfoData = new FileInfoData
        {
            Content = fileContentData,
            Size = data.Length,
            Type = fileType.Value,
            UrlId = await GenerateFileUrlIdAsync(),
            OriginalUrl = url
        };

        databaseContext.FileInfos.Add(fileInfoData);

        await databaseContext.SaveChangesAsync();

        databaseContext.Entry(fileInfoData).State = EntityState.Detached;
        databaseContext.Entry(fileContentData).State = EntityState.Detached;
        fileInfoData.Content = null;

        return contentMapper.Map(fileInfoData);
    }

    public async Task<ValueResult<FileInfo>> GetByUrlIdAsync(string urlId)
    {
        var data = await databaseContext.FileInfos.AsNoTracking().Include(x => x.Content).SingleOrDefaultAsync(x => x.UrlId == urlId);

        if (data == null)
        {
            return ResultMessage.NotFound("File");
        }

        var model = contentMapper.Map(data);

        return model;
    }

    public async Task<Result> DeleteAsync(int fileInfoId)
    {
        var data = await databaseContext.FileInfos.Include(x => x.Content).SingleOrDefaultAsync(x => x.Id == fileInfoId);

        if (data == null)
        {
            return ResultMessage.NotFound("File");
        }

        databaseContext.FileInfos.Remove(data);
        databaseContext.FileContents.Remove(data.Content!);

        await databaseContext.SaveChangesAsync();

        return Result.Success();
    }

    private async Task<string> GenerateFileUrlIdAsync()
    {
        const string characters = "abcdefghijklmnopqrstuvwxyz1234567890";
        var random = new Random();

        string urlId;

        do
        {
            urlId = new string(Enumerable.Repeat('\0', 5).Select(x => characters[random.Next(characters.Length)]).ToArray());
        }
        while (await databaseContext.FileInfos.AnyAsync(x => x.UrlId == urlId));

        return urlId;
    }
}
