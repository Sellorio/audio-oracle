using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Library.Results.Messages;
using Sellorio.AudioOracle.ServiceInterfaces.Content;
using System.IO;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Services.Content;

internal class DataFileService : IDataFileService
{
    public async Task<Result> PostDataFileAsync(string fileName, long fileSize, Stream contents)
    {
        var fileNameLower = fileName.ToLower();
        int maxLength;

        switch (fileNameLower)
        {
            case "cookies-youtube.txt":
                fileName = fileNameLower;
                maxLength = 200_000;
                break;
            default:
                return ResultMessage.Error("Unexpected data file name.");
        }

        if (fileSize > maxLength)
        {
            return ResultMessage.Error("File is larger than expected.");
        }

        using var fs = new FileStream(Path.Combine("/data", fileName), FileMode.Create, FileAccess.Write);
        await contents.CopyToAsync(fs);

        return Result.Success();
    }
}
