// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

namespace FMLab.Aspnet.URLShortener.Business.Shared.Result;

public class Result
{
    public bool IsSuccess { get; private set; }
    public bool IsFailure { get { return !IsSuccess; } }
    public string? Message { get; private set; }
    public ResultErrorType? ErrorType { get; private set; }
    protected Result(bool success, string message)
    {
        IsSuccess = success;
        Message = message;
    }
    public static Result Failure(string message, ResultErrorType errorType = ResultErrorType.None)
    {
        return new Result(false, message);
    }
    public static Result Success(string message = default!)
    {
        return new Result(true, message);
    }
}

public class Result<TData>
    where TData : class
{
    public bool IsSuccess { get; private set; }
    public bool IsFailure { get { return !IsSuccess; } }
    public string? Message { get; private set; }
    public TData? Data { get; private set; }
    public ResultErrorType? ErrorType { get; private set; }

    private Result(bool success, string message, TData data)
    {
        IsSuccess = success;
        Message = message;
        Data = data;
    }
    public static Result<TData> Failure (string message, ResultErrorType errorType = ResultErrorType.None, TData data = default!)
    {
        return new Result<TData>(false, message, data);
    }
    public static Result<TData> Success(TData data, string message = default!)
    {
        return new Result<TData>(true, message, data);
    }
}

public enum ResultErrorType
{
    None,
    NotFound,
    Confict,
    Other
}