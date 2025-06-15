// MIT License
// Copyright (c) 2021 Pavel Anpin

using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("APES.MAUI.Droid")]
[assembly: InternalsVisibleTo("APES.MAUI.iOS")]
[assembly: InternalsVisibleTo("APES.MAUI.UWP")]
[assembly: InternalsVisibleTo("APES.MAUI.Mac")]

namespace APES.MAUI
{
    internal static class Logger
    {
        public static bool DebugEnabled { get; set; } =
#if DEBUG
                true;

#else
                false;
#endif
        public static void Debug(string format, params object[] parameters)
        {
            if (DebugEnabled)
            {
                DiagnosticLog("DEBUG " + format, parameters);
            }
        }

        public static void Error(string format, params object[] parameters) => DiagnosticLog("ERROR " + format, parameters);

        public static void Error(Exception exception) => Error($"{exception.Message}{Environment.NewLine}{exception.Source}{Environment.NewLine}{exception.StackTrace}");

        private static void DiagnosticLog(string format, params object[] parameters)
        {
            string formatWithHeader = " APES.MAUI " + DateTime.Now.ToString("MM-dd H:mm:ss.fff ") + format;
#if DEBUG
            System.Diagnostics.Debug.WriteLine(formatWithHeader, parameters);
#else
            Console.WriteLine(formatWithHeader, parameters);
#endif
        }
    }
}
