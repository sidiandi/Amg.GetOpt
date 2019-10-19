using Serilog;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Amg
{
    public class TestBase
    {
        protected TestBase()
        {

        }

        static TestBase()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console(standardErrorFromLevel: Serilog.Events.LogEventLevel.Error)
                .CreateLogger();
        }

        public static TimeSpan MeasureTime(Action a)
        {
            var stopwatch = Stopwatch.StartNew();
            a();
            return stopwatch.Elapsed;
        }

        public static TimeSpan MeasureTime(Func<Task> a)
        {
            var stopwatch = Stopwatch.StartNew();
            a().Wait();
            return stopwatch.Elapsed;
        }
    }
}
