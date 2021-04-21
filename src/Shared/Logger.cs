using System;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("APES.UI.XF.Droid")]
[assembly: InternalsVisibleTo("APES.UI.XF.iOS")]
[assembly: InternalsVisibleTo("APES.UI.XF.UWP")]
[assembly: InternalsVisibleTo("APES.UI.XF.Mac")]
namespace APES.UI.XF
{
    internal static class Logger
    {

        public static void Error(string format, params object[] parameters)
        {
            DiagnosticLog("ERROR " + format, parameters);
        }

        public static void Error(Exception exception)
        {
            Error($"{exception.Message}{Environment.NewLine}{exception.Source}{Environment.NewLine}{exception.StackTrace}");
        }
        private static void DiagnosticLog(string format, params object[] parameters)
        {
            var formatWithHeader = " APES.UI.XF " + DateTime.Now.ToString("MM-dd H:mm:ss.fff ") + format;
#if DEBUG
            System.Diagnostics.Debug.WriteLine(formatWithHeader, parameters);
#else
            Console.WriteLine(DateTime.Now.ToString(formatWithHeader, parameters);
#endif
        }
    }
}
