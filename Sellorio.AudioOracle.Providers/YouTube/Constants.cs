namespace Sellorio.AudioOracle.Providers.YouTube;

internal static class Constants
{
    public const string ProviderName = "YouTube";
    public const string CookiesPath = "/data/cookies-youtube.txt";
    public const string FfmpegPath = "/data/ffmpeg";
    public const string UnregisteredArtistIdPrefix = "unreg:";
    public const string UnregisteredArtistIdFormat = UnregisteredArtistIdPrefix + "{{ALBUM_ID}}:{{ARTIST_NAME}}";
    public const string YouTubeIdRegex = "[a-zA-Z0-9_-]+";

    public const string SearchBySongsParams = "EgWKAQIIAWoQEAMQBBAJEAoQBRAREBAQFQ%3D%3D";
    public const string SearchByAlbumsParams = "EgWKAQIYAWoQEAMQBBAJEAoQBRAREBAQFQ%3D%3D";
}
