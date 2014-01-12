using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bloggy.Client.Web.Infrastructure
{
    internal static class AsyncHelper
    {
        private static readonly TaskFactory MyTaskFactory = new TaskFactory(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            return MyTaskFactory.StartNew<Task<TResult>>(func).Unwrap<TResult>().GetAwaiter().GetResult();
        }

        public static void RunSync(Func<Task> func)
        {
            MyTaskFactory.StartNew<Task>(func).Unwrap().GetAwaiter().GetResult();
        }
    }
}