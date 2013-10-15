using NLog;
using System;

namespace Bloggy.Client.Web.Infrastructure.Logging
{
    /// <remarks>
    /// Refer to http://dotnetdarren.wordpress.com/2010/07/28/logging-in-mvc-part-3-%E2%80%93-nlog/
    /// </remarks>
    public class NLogLogger : IMvcLogger
    {
        private readonly Logger _logger;

        public NLogLogger()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }

        public void Info(string message)
        {
            _logger.Info(message);
        }

        public void Warn(string message)
        {
            _logger.Warn(message);
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Error(string message)
        {
            _logger.Error(message);
        }

        public void Error(Exception exception)
        {
            Error(exception.ToString());
        }

        public void Error(string message, Exception exception)
        {
            _logger.ErrorException(message, exception);
        }

        public void Fatal(string message)
        {
            _logger.Fatal(message);
        }

        public void Fatal(Exception exception)
        {
            Fatal(exception.ToString());
        }
    }
}