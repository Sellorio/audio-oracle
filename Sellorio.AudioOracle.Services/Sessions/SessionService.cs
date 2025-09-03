using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sellorio.AudioOracle.Data;
using Sellorio.AudioOracle.Data.Sessions;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Library.Results.Messages;
using Sellorio.AudioOracle.Models;

namespace Sellorio.AudioOracle.Services.Sessions;

internal class SessionService(DatabaseContext databaseContext, SessionState sessionState) : ISessionService
{
    private static readonly TimeSpan _sessionExpiryTime = TimeSpan.FromDays(30);

    public async Task<Result> InvalidateAllSessionsAsync()
    {
        var sessions = await databaseContext.Sessions.ToArrayAsync();
        databaseContext.Sessions.RemoveRange(sessions);
        await databaseContext.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<ValueResult<string>> LoginAsync(string password)
    {
        sessionState.IsLoggedIn = false;
        sessionState.SessionToken = null;

        if (await IsCorrectPasswordAsync(password))
        {
            var now = DateTimeOffset.Now;

            var session = new SessionData
            {
                CreatedAt = now,
                LastAccessedAt = now,
                Guid = Guid.NewGuid()
            };

            databaseContext.Sessions.Add(session);
            await databaseContext.SaveChangesAsync();

            sessionState.IsLoggedIn = true;
            sessionState.SessionToken = session.Guid.ToString();

            return sessionState.SessionToken;
        }

        return ResultMessage.Error(Errors.IncorrectPassword);
    }

    public async Task<Result> LogoutAsync()
    {
        if (!sessionState.IsLoggedIn)
        {
            return ResultMessage.Error("Cannot log out if there is no active session.");
        }

        var guid = Guid.Parse(sessionState.SessionToken!);
        var session = await databaseContext.Sessions.SingleOrDefaultAsync(x => x.Guid == guid);

        // not sure how this can happen but it's best to avoid an edge case 500 error
        if (session == null)
        {
            return Result.Success();
        }

        databaseContext.Sessions.Remove(session);
        await databaseContext.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> UseSessionAsync(string sessionToken)
    {
        var sessionResult = await GetAndValidateSessionAsync(sessionToken);

        if (!sessionResult.WasSuccess)
        {
            return sessionResult.AsResult();
        }

        sessionResult.Value!.LastAccessedAt = DateTimeOffset.Now;
        await databaseContext.SaveChangesAsync();

        return Result.Success();
    }

    public Task<bool> IsCorrectPasswordAsync(string password)
    {
        var expectedPassword = Environment.GetEnvironmentVariable(EnvironmentVariables.AdminPassword);
        var passwordsMatch = expectedPassword == password;
        return Task.FromResult(passwordsMatch);
    }

    private async Task<ValueResult<SessionData>> GetAndValidateSessionAsync(string sessionToken)
    {
        if (string.IsNullOrEmpty(sessionToken))
        {
            return ResultMessage.Error(Errors.MissingSessionId);
        }

        if (!Guid.TryParse(sessionToken, out var sessionGuid))
        {
            return ResultMessage.Error(Errors.IncorrectSessionId);
        }

        var session = await databaseContext.Sessions.SingleOrDefaultAsync(x => x.Guid == sessionGuid);

        if (session == null)
        {
            return ResultMessage.Error(Errors.IncorrectSessionId);
        }

        if (session.LastAccessedAt + _sessionExpiryTime < DateTimeOffset.Now)
        {
            databaseContext.Sessions.Remove(session);
            await databaseContext.SaveChangesAsync();

            return ResultMessage.Error(Errors.SessionExpired);
        }

        return session;
    }
}
