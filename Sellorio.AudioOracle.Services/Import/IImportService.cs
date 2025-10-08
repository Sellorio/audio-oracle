using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Services.Import;
internal interface IImportService
{
    Task<bool> TryImportAsync(string sourceId, string outputFilename);
}