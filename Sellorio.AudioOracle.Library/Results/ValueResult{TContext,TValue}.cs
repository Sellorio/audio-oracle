using Sellorio.AudioOracle.Library.Results.Json;
using Sellorio.AudioOracle.Library.Results.Messages;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace Sellorio.AudioOracle.Library.Results;

[JsonConverter(typeof(ResultJsonConverter))]
public class ValueResult<TContext, TValue> : IResult
{
    public bool WasSuccess { get; }

    public IReadOnlyList<ResultMessage> Messages { get; }
    public TValue Value { get; }

    private ValueResult(ImmutableArray<ResultMessage> messages, TValue value)
    {
        WasSuccess = !messages.Any(x => x.Severity is ResultMessageSeverity.Critical or ResultMessageSeverity.Error);
        Messages = messages;
        Value = value;
    }

    public Result<TNewContext> Select<TNewContext>(Expression<Func<TContext, TNewContext>> pathToChild)
    {
        return AsResult().Select(pathToChild);
    }

    public Result<TNewContext> SelectOut<TNewContext>(Expression<Func<TNewContext, TContext>> pathFromParent)
    {
        return AsResult().SelectOut(pathFromParent);
    }

    public Result<TContext> Exclude<TNewContext>(params Expression<Func<TContext, TNewContext>>[] pathToChildren)
    {
        return AsResult().Exclude(pathToChildren);
    }

    public Result<TContext> AsResult()
    {
        return new Result<TContext>((ImmutableArray<ResultMessage>)Messages);
    }

    public static ValueResult<TContext, TValue> Success(TValue value, params ResultMessage[] messages)
    {
        messages ??= [];

        if (messages.Any(x => x.Severity is ResultMessageSeverity.Critical or ResultMessageSeverity.Error))
        {
            throw new ArgumentException("Cannot create a successful result with Critical or Error messages.", nameof(messages));
        }

        return new ValueResult<TContext, TValue>([.. messages], value);
    }

    public static ValueResult<TContext, TValue> Failure(params ResultMessage[] messages)
    {
        return Failure((IEnumerable<ResultMessage>)messages);
    }

    public static ValueResult<TContext, TValue> Failure(IEnumerable<ResultMessage> messages)
    {
        ArgumentNullException.ThrowIfNull(messages);

        if (!messages.Any(x => x.Severity is ResultMessageSeverity.Critical or ResultMessageSeverity.Error))
        {
            throw new ArgumentException("Cannot create a failed result without any Critical or Error messages.", nameof(messages));
        }

        return new ValueResult<TContext, TValue>([.. messages], default);
    }

    public static ValueResult<TContext, TValue> operator |(ValueResult<TContext, TValue> left, Result<TContext> right)
    {
        return new ValueResult<TContext, TValue>(Enumerable.Concat(left.Messages, right.Messages).ToImmutableArray(), left.Value);
    }

    public static ValueResult<TContext, TValue> operator |(ValueResult<TContext, TValue> left, Result right)
    {
        return new ValueResult<TContext, TValue>(Enumerable.Concat(left.Messages, right.Messages).ToImmutableArray(), left.Value);
    }

    public static ValueResult<TContext, TValue> operator |(Result<TContext> left, ValueResult<TContext, TValue> right)
    {
        return new ValueResult<TContext, TValue>(Enumerable.Concat(left.Messages, right.Messages).ToImmutableArray(), right.Value);
    }

    public static ValueResult<TContext, TValue> operator |(Result left, ValueResult<TContext, TValue> right)
    {
        return new ValueResult<TContext, TValue>(Enumerable.Concat(left.Messages, right.Messages).ToImmutableArray(), right.Value);
    }

    public static implicit operator ValueResult<TContext, TValue>(ResultMessage message)
    {
        return Failure(message);
    }

    public static implicit operator ValueResult<TContext, TValue>(TValue value)
    {
        return Success(value);
    }
}