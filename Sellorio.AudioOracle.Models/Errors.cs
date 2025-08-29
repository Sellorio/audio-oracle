namespace Sellorio.AudioOracle.Models;

public static class Errors
{
    public const string MissingSessionId = "Missing session ID. Please log in to perform this action.";
    public const string IncorrectSessionId = "Incorrect session ID.";
    public const string SessionExpired = "Session has expired. Please log in again.";
    public const string IncorrectPassword = "Incorrect password was provided.";
}
