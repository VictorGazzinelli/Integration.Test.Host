using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Integration.Test.Host
{
    public interface ITestConfiguration : IAsyncLifetime
    {
        Task ConfigureTestServicesAsync(IServiceCollection services, IConfiguration configuration);
    }
}
