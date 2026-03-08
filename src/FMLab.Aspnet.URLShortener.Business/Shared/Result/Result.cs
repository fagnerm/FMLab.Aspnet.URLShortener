// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

namespace FMLab.Aspnet.URLShortener.Business.Shared.Result;
public interface IResultBase
{
    bool IsSuccess { get; }
    string? Error { get; }
    ResultType Type { get; }
}

public interface IResultBase<TSelf> : IResultBase
    where TSelf : class, IResultBase<TSelf>
{
    static abstract TSelf Validation(string? error);
}

public class Result<TResult> : IResultBase<Result<TResult>>
    where TResult : class
{
    public TResult? Data { get; private set; }

    public bool IsSuccess { get; private set; }
    public string? Error { get; private set; }
    public ResultType Type { get; private set; }

    private Result(ResultType type = ResultType.Success)
    {
        IsSuccess = type is ResultType.Success or ResultType.NoContent;
        Type = type;
    }

    private Result(string? error, ResultType type)
    {
        Error = error;
        Type = type;
    }

    public static Result<TResult> Success(TResult? data = default)
    {
        return new Result<TResult>(ResultType.Success) { Data = data };
    }

    public static Result<TResult> NoContent()
    {
        return new Result<TResult>(ResultType.NoContent);
    }

    public static Result<TResult> NotFound(string? error)
    {
        return new Result<TResult>(error, ResultType.NotFound);
    }

    public static Result<TResult> Validation(string? error)
    {
        return new Result<TResult>(error, ResultType.Validation);
    }

    public static Result<TResult> Domain(string? error)
    {
        return new Result<TResult>(error, ResultType.Domain);
    }

    public static Result<TResult> Conflict(string? error)
    {
        return new Result<TResult>(error, ResultType.Conflict);
    }
}

public class NoOutput { }

public enum ResultType
{
    Success,
    NoContent,
    NotFound,
    Validation,
    Domain,
    Conflict
}
