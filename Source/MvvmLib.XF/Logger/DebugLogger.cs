using System;
using System.Diagnostics;
using System.Globalization;

namespace MvvmLib.Logger
{
    public class DebugLogger : ILogger
    {
        public void Log(string message, Category category, Priority priority)
        {
            string messageToLog = string.Format(CultureInfo.InvariantCulture, "{1}: {2}. Priority: {3}. Timestamp:{0:u}.", DateTime.Now, category.ToString().ToUpper(), message, priority);

            Debug.WriteLine(messageToLog);
        }
    }
}
