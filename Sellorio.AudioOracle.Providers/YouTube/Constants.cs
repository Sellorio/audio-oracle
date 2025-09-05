namespace Sellorio.AudioOracle.Providers.YouTube;

internal static class Constants
{
    public const string ProviderName = "YouTube";
    public const string CookiesPath = "/data/cookies-youtube.txt";
    public const string UnregisteredArtistIdPrefix = "unreg:";
    public const string UnregisteredArtistIdFormat = UnregisteredArtistIdPrefix + "{{ALBUM_ID}}:{{ARTIST_NAME}}";
}
