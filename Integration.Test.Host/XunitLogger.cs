using System;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Integration.Test.Host
{
    public class XunitLogger<T> : ILogger<T>
    {
        private readonly ITestOutputHelper output;

        public XunitLogger(ITestOutputHelper output)
        {
            this.output = output;
        }

        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            output.WriteLine($"[{logLevel}] {formatter(state, exception)}");
            if (exception != null)
                output.WriteLine(exception.ToString());
        }
    }
}
