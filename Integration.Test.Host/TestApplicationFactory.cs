using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Integration.Test.Host
{
    public abstract class TestApplicationFactory<TProgram> : WebApplicationFactory<TProgram>, IAsyncLifetime where TProgram : class
    {
        protected abstract ITestConfiguration TestConfiguration { get; }
        private ITestOutputHelper _testOutputHelper;

        public void SetTestOutputHelper(ITestOutputHelper testOutputHelper) => _testOutputHelper = testOutputHelper;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");
            builder.ConfigureLogging(loggingBuilder =>
            {
                if (_testOutputHelper != null)
                    loggingBuilder.AddProvider(new XunitLoggerProvider(_testOutputHelper));
            });
            builder.ConfigureServices(async (webHostBuilderContext, services) =>
            {
                await TestConfiguration.ConfigureTestServicesAsync(services, webHostBuilderContext.Configuration);
            });
        }

        public T GetService<T>() => (T)Services.GetService(typeof(T));

        public ValueTask InitializeAsync()
            => TestConfiguration.InitializeAsync();
    }
}
