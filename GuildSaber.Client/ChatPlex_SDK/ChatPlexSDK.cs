using System;

namespace CP_SDK;

public class ChatPlexSDK
{
    public static Logging.ILogger Logger = new CustomLogger();
}

public class CustomLogger : CP_SDK.Logging.ILogger
{
    protected override void LogImplementation(ELogType p_Type, string    p_Data)
    {
        switch (p_Type)
        {
            case ELogType.Error:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(p_Data);
                Console.ResetColor();
                return;
            case ELogType.Warning:
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(p_Data);
                Console.ResetColor();
                return;
            case ELogType.Info:
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(p_Data);
                Console.ResetColor();
                return;
            case ELogType.Debug:
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(p_Data);
                Console.ResetColor();
                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(p_Type), p_Type, null);
        }
    }

    protected override void LogImplementation(ELogType p_Type, Exception p_Data)
    {
        switch (p_Type)
        {
            case ELogType.Error:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(p_Data);
                Console.ResetColor();
                return;
            case ELogType.Warning:
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(p_Data);
                Console.ResetColor();
                return;
            case ELogType.Info:
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(p_Data);
                Console.ResetColor();
                return;
            case ELogType.Debug:
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(p_Data);
                Console.ResetColor();
                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(p_Type), p_Type, null);
        }
    }
}
