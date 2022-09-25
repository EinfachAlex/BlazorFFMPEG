using System.Runtime.CompilerServices;
using EinfachAlex.Utils.WebRequest;

namespace BlazorFFMPEG.Backend.Modules.Logging;

public static class LoggerMessageDefinitions
{
    public const string PREFIX_WEBREQUEST = "[WEBREQUEST] ";

    private static readonly Action<ILogger, ERequestTypes, string, Exception?> endpointRequestLogMessage
        = LoggerMessage.Define<ERequestTypes, string>(LogLevel.Information, 0, "[WEBREQUEST] {Method} {Endpoint}");

    public static void logEndpointRequestLogMessage(this ILogger logger, string endpoint, ERequestTypes method)
    {
        endpointRequestLogMessage(logger, method, endpoint, null);
    }
    
    
    private static readonly Action<ILogger, long, Exception?> elapsedTimeLogMessage
        = LoggerMessage.Define<long>(LogLevel.Debug, 0, "=> {elapsedTime} ms");
    
    public static void logElapsedTimeLogMessage(this ILogger logger, long elapsedTime)
    {
        elapsedTimeLogMessage(logger, elapsedTime, null);
    }
    
    
    private static readonly Action<ILogger, string, long, string, Exception?> genericMessage_string_number_string
        = LoggerMessage.Define<string, long, string>(LogLevel.Information, 0, "=> {text} {variable} {text}");
    
    public static void logGenericMessage(this ILogger logger, string msg1, long msg2, string msg3)
    {
        genericMessage_string_number_string(logger, msg1, msg2, msg3, null);
    }
    
    
    private static readonly Action<ILogger, string, string, Exception?> genericMessage_string_string
        = LoggerMessage.Define<string, string>(LogLevel.Information, 0, "=> {msg1} {msg2}");
    
    public static void logGenericMessage(this ILogger logger, string msg1, string msg2, LogLevel logLevel = LogLevel.Information)
    {
        genericMessage_string_string(logger, msg1, msg2, null);
    }
    
    
    private static readonly Action<ILogger, string, Exception?> encoderFoundLogMessage
        = LoggerMessage.Define<string>(LogLevel.Information, 0, "=> Found encoder {encoder}");
    
    public static void logEncoderFound(this ILogger logger, string encoder)
    {
        encoderFoundLogMessage(logger, encoder, null);
    }
    
    
    private static readonly Action<ILogger, string, Exception?> parametersMissingLogMessage
        = LoggerMessage.Define<string>(LogLevel.Information, 0, "Error: One or more parameters are missing{_}");
    
    public static void logParametersMissing(this ILogger logger)
    {
        parametersMissingLogMessage(logger, null, null);
    }
}