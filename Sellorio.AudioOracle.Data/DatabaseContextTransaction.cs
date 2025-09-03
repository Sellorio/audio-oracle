using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace Sellorio.AudioOracle.Data;

public class DatabaseContextTransaction
{
    private readonly IDbContextTransaction? _transaction;

    public bool IsComplete { get; private set; }

    internal DatabaseContextTransaction(IDbContextTransaction? transaction)
    {
        _transaction = transaction;
    }

    public async Task CommitAsync()
    {
        if (IsComplete)
        {
            throw new InvalidOperationException("The transaction has already been committed/rolledback.");
        }

        IsComplete = true;

        if (_transaction != null)
        {
            await _transaction.CommitAsync();
        }
    }

    public async Task RollbackAsync()
    {
        if (IsComplete)
        {
            throw new InvalidOperationException("The transaction has already been committed/rolledback.");
        }

        IsComplete = true;

        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
        }
    }
}
