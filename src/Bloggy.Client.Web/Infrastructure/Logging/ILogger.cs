using System;

namespace Bloggy.Client.Web.Infrastructure.Logging
{
    public interface IMvcLogger
    {
        void Info(string message);
        void Warn(string message);
        void Debug(string message);
        void Error(string message);
        void Error(Exception exception);
        void Error(string message, Exception exception);
        void Fatal(string message);
        void Fatal(Exception exception);
    }
}