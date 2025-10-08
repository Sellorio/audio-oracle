using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Services.Import;

internal class ImportService : IImportService
{
    private static readonly IList<ManifestAlbum> _manifest;

    static ImportService()
    {
        if (File.Exists("/import/ytm-manifest.yml"))
        {
            var deserializer =
                new YamlDotNet.Serialization.DeserializerBuilder()
                    .WithNamingConvention(YamlDotNet.Serialization.NamingConventions.CamelCaseNamingConvention.Instance)
                    .IgnoreUnmatchedProperties()
                    .Build();

            _manifest = deserializer.Deserialize<IList<ManifestAlbum>>(File.ReadAllText("/import/ytm-manifest.yml"));
        }
        else
        {
            _manifest = [];
        }
    }

    public Task<bool> TryImportAsync(string sourceId, string outputFilename)
    {
        foreach (var album in _manifest)
        {
            foreach (var track in album.Tracks)
            {
                if (track.YouTubeId == sourceId)
                {
                    var filename = Path.Combine("/import", album.FolderName, track.FileName);

                    if (File.Exists(filename))
                    {
                        var outputDir = Path.GetDirectoryName(outputFilename)!;

                        if (!Directory.Exists(outputDir))
                        {
                            Directory.CreateDirectory(outputDir);
                        }

                        File.Copy(filename, outputFilename);
                        return Task.FromResult(true);
                    }

                    return Task.FromResult(false);
                }
            }
        }

        return Task.FromResult(false);
    }
}
