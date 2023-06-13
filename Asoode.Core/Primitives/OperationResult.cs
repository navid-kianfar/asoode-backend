using System;
using Asoode.Core.Primitives.Enums;

namespace Asoode.Core.Primitives;

public class OperationResult<T>
{
    private OperationResult()
    {
        Status = OperationResultStatus.Pending;
    }

    public T Data { get; set; }
    public Exception Exception { get; set; }
    public OperationResultStatus Status { get; set; }

    public static OperationResult<T> Captcha()
    {
        return new OperationResult<T>
        {
            Status = OperationResultStatus.Captcha
        };
    }

    public static OperationResult<T> Duplicate()
    {
        return new OperationResult<T>
        {
            Status = OperationResultStatus.Duplicate
        };
    }

    public static OperationResult<T> Expired()
    {
        return new OperationResult<T>
        {
            Status = OperationResultStatus.Expire
        };
    }

    public static OperationResult<T> Failed(Exception exception = null)
    {
        return new OperationResult<T>
        {
            Exception = exception,
            Status = OperationResultStatus.Failed
        };
    }

    public static OperationResult<T> NotFound()
    {
        return new OperationResult<T>
        {
            Status = OperationResultStatus.NotFound
        };
    }

    public static OperationResult<T> OverCapacity()
    {
        return new OperationResult<T>
        {
            Status = OperationResultStatus.OverCapacity
        };
    }

    public static OperationResult<T> Rejected()
    {
        return new OperationResult<T>
        {
            Status = OperationResultStatus.Rejected
        };
    }

    public static OperationResult<T> Success(T data = default)
    {
        return new OperationResult<T>
        {
            Data = data,
            Status = OperationResultStatus.Success
        };
    }

    public static OperationResult<T> UnAuthorized()
    {
        return new OperationResult<T>
        {
            Status = OperationResultStatus.UnAuthorized
        };
    }

    public static OperationResult<T> Validation()
    {
        return new OperationResult<T>
        {
            Status = OperationResultStatus.Validation
        };
    }
}