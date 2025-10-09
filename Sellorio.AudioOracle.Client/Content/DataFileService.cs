using Sellorio.AudioOracle.Client.Internal;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.ServiceInterfaces.Content;
using System.IO;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Client.Content;

internal class DataFileService(IRestClient restClient) : IDataFileService
{
    public Task<Result> PostDataFileAsync(string fileName, long fileSize, Stream contents)
    {
        return restClient.Post($"/df{new { fileName }}", contents).ToResult();
    }
}
