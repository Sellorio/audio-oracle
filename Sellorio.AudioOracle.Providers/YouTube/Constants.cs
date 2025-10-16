using System.Text.RegularExpressions;

namespace Sellorio.AudioOracle.Providers.YouTube;

internal static partial class Constants
{
    public const string ProviderName = "YouTube";
    public const string CookiesPath = "/data/cookies-youtube.txt";
    public const string FfmpegPath = "/data/ffmpeg";
    public const string UnregisteredArtistIdPrefix = "unreg:";
    public const string UnregisteredArtistIdFormat = UnregisteredArtistIdPrefix + "{{ALBUM_ID}}:{{ARTIST_NAME}}";
    public const string YouTubeIdRegex = "[a-zA-Z0-9_-]+";

    public const string SearchBySongsParams = "EgWKAQIIAWoOEAMQBBAJEAoQEBAVEBE%3D";
    public const string SearchByAlbumsParams = "EgWKAQIYAWoOEAMQBBAJEAoQEBAVEBE%3D";


    // Supported Uri Formats:
    // https://music.youtube.com/watch?v=Cqp-dB7GVI8&list=PLIr8oAMYGij0QrgUfzLyqbwrHfaBtXL1w
    // https://youtu.be/Cqp-dB7GVI8
    // https://www.youtube.com/watch?v=Cqp-dB7GVI8
    [GeneratedRegex(@"^https:\/\/(?:music\.youtube\.com\/watch\?v=|youtu\.be\/|www\.youtube\.com\/watch\?v=)([a-zA-Z0-9_-]+)[&a-zA-Z0-9=_]*$", RegexOptions.IgnoreCase)]
    public static partial Regex TrackUriRegex();

    [GeneratedRegex(@"https:\/\/(?:music\.youtube\.com\/playlist\?list=|www\.youtube\.com\/playlist\?list=)([a-zA-Z0-9_-]+)[&a-zA-Z0-9=_]*$")]
    public static partial Regex AlbumUriRegex();
}
