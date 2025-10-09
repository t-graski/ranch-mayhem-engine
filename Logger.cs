using System;
using ranch_mayhem_engine.UI;

namespace ranch_mayhem_engine;

public static class Logger
{
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

        if (level <= RanchMayhemEngine.LogLevel)
        {
            Console.WriteLine($"{prefix}{message}\x1b[39m");
            // write to rm.log
            try
            {
                File.AppendAllText("rm.log", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {level.ToString().ToUpper(),-8} - {message}\n");
            }
            catch
            {
                // ignore
            }
        }
    }
}