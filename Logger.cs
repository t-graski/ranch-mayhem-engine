using System;

namespace ranch_mayhem_engine;

public static class Logger
{
    public enum LogLevel
    {
        Info,
        Warning,
        Error,
        Internal
    }

    public static void Log(string message, LogLevel level = LogLevel.Info)
    {
        var prefix = level switch
        {
            LogLevel.Info => "\x1b[92mINFO     - ",
            LogLevel.Warning => "\x1b[93mWARNING  - ",
            LogLevel.Error => "\x1b[91mERROR    - ",
            LogLevel.Internal => "\x1b[95mINTERNAL - ",
            _ => "\x1b[39m"
        };

        Console.WriteLine($"{prefix}{message}\x1b[39m");
    }
}