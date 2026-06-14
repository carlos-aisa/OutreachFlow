using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using OutreachFlow.Web;

namespace OutreachFlow.IntegrationTests.Web;

internal sealed class OutreachFlowWebFactory : WebApplicationFactory<WebAssemblyMarker>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, configurationBuilder) =>
        {
            var overrides = new Dictionary<string, string?>
            {
                ["OutreachFlowApi:BaseUrl"] = "http://localhost",
                ["Hosting:UseHttpsRedirection"] = "false",
                ["Logging:LogLevel:Default"] = "Warning"
            };

            configurationBuilder.AddInMemoryCollection(overrides);
        });
    }
}
