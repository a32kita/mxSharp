using System;
using System.Net;

namespace MxSharp;

public class MixiException : WebException
{
    public MixiException(string apiError)
        : base($"API Error: {apiError}")
    {
        ApiError = apiError;
    }

    public MixiException(string apiError, Exception innerException)
        : base($"API Error: {apiError}", innerException)
    {
        ApiError = apiError;
    }

    public string ApiError { get; }
}