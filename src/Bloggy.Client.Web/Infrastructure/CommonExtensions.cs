using System;

namespace Bloggy.Client.Web.Infrastructure
{
    public static class CommonExtensions
    {
        public static DateTime ToUtcToday(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
        }
    }
}