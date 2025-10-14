using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace AudioOracleCompanion;

internal class Config
{
    public static string DataPath { get; }

    static Config()
    {
        var deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
        var model = deserializer.Deserialize<Model>(File.ReadAllText(Path.Combine(Path.GetDirectoryName(typeof(Config).Assembly.Location)!, "config.yml")));

        DataPath = model.DataPath;
    }

    private class Model
    {
        public required string DataPath { get; init; }
    }
}
