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
    private Result(bool success, string message, ResultErrorType resultError)
    {
        IsSuccess = success;
        Message = message;
        ErrorType = resultError;
    }
    public static Result Failure(string message, ResultErrorType errorType = ResultErrorType.None)
    {
        return new Result(false, message, errorType);
    }
    public static Result Success(string message = default!)
    {
        return new Result(true, message, ResultErrorType.None);
    }
}

public class Result<TData>
{
    public bool IsSuccess { get; private set; }
    public bool IsFailure { get { return !IsSuccess; } }
    public string? Message { get; private set; }
    public TData? Data { get; private set; }
    public ResultErrorType? ErrorType { get; private set; }

    private Result(bool success, string message, TData data, ResultErrorType errorType)
    {
        IsSuccess = success;
        Message = message;
        Data = data;
        ErrorType = errorType;
    }
    public static Result<TData> Failure(string message, ResultErrorType errorType = ResultErrorType.None, TData data = default!)
    {
        return new Result<TData>(false, message, data, errorType);
    }
    public static Result<TData> Success(TData data, string message = default!)
    {
        return new Result<TData>(true, message, data, ResultErrorType.None);
    }
}

public enum ResultErrorType
{
    None,
    NotFound,
    Conflict,
    Other
}