using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace SmartSnsPublisher.Utility
{
    public static class HelperLogger
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static void Debug(string message)
        {
            _logger.Debug(message);
        }

        public static void Debug(string format, params object[] args)
        {
            var message = string.Format(format, args);
            _logger.Debug(message);

        }

        public static void DebugException(string message, Exception exception)
        {
            _logger.DebugException(message, exception);
        }
    }
}
