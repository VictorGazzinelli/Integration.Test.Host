using System;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Integration.Test.Host
{
    public class XunitLoggerProvider : ILoggerProvider
    {
        private sealed class XunitInternalLogger : ILogger
        {
            private readonly string _category;
            private readonly ITestOutputHelper _output;
            private readonly IExternalScopeProvider _scopes;

            public XunitInternalLogger(string category, ITestOutputHelper output, IExternalScopeProvider scopes)
            {
                _category = category;
                _output = output;
                _scopes = scopes;
            }

            public IDisposable BeginScope<TState>(TState state) =>
                _scopes.Push(state);

            public bool IsEnabled(LogLevel logLevel) => 
                true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                if (!IsEnabled(logLevel)) return;

                string message = null;
                try
                {
                    if (_output != null)
                    {
                        message = $"[{DateTime.Now:HH:mm:ss.fff}] " +
                                            $"[{logLevel}] {_category}: {formatter(state, exception)}";
                        _output.WriteLine(message);

                        if (exception is not null)
                            _output.WriteLine(exception.ToString());
                    }
                }
                catch (Exception)
                {
                    // Ignore any exceptions from logging to avoid breaking the main flow
                }
            }
        }

        private readonly ITestOutputHelper _output;
        private readonly LoggerExternalScopeProvider _scopeProvider = new();

        public XunitLoggerProvider(ITestOutputHelper output) => 
            _output = output;

        public ILogger CreateLogger(string categoryName) =>
            new XunitInternalLogger(categoryName, _output, _scopeProvider);

        public void Dispose()
        {
            // Nothing to dispose
        }
    }
}
