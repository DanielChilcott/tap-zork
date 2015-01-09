using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Zork
{
    public class Logging
    {
        static string _path;

        public static void SetFilePath(string path)
        {
            _path = path;
        }

        public static void LogEvent(string text, Exception ex = null)
        {
            text = string.Format(
                "{0} {1}\r\n{2}",
                DateTime.UtcNow.ToString(),
                text,
                ex != null ? generateExceptionLog(ex) : "");

#if DEBUG 
            System.Diagnostics.Debug.WriteLine(text);
#endif

            File.AppendAllText(_path, text);
        }

        private static string generateExceptionLog(Exception ex)
        {
            StringBuilder sb = new StringBuilder();

            int depth = 0;

            while (ex != null)
            {
                sb.AppendLine(new string(' ', depth * 2) + ex.Message);
                var stack = ex.StackTrace;
                if (stack != null) stack = stack.Replace("\r\n", new string(' ', depth * 2) + "\r\n");
                sb.AppendLine(stack);
                sb.AppendLine();
                depth++;
                ex = ex.InnerException;
            }

            sb.AppendLine();
            sb.AppendLine();
            return sb.ToString();

        }

    }
}