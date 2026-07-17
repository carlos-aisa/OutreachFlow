using System.Text.Json;
using FluentAssertions;

namespace OutreachFlow.IntegrationTests.Configuration;

public sealed class DevelopmentHostingConfigurationTests
{
    [Theory]
    [InlineData("src/OutreachFlow.Api/appsettings.Development.json")]
    [InlineData("src/OutreachFlow.Web/appsettings.Development.json")]
    public async Task ShouldDisableHttpsRedirectionForTheDefaultHttpDevelopmentProfiles(string configurationPath)
    {
        var repositoryRoot = FindRepositoryRoot();
        var configuration = await File.ReadAllTextAsync(Path.Combine(repositoryRoot, configurationPath));

        using var document = JsonDocument.Parse(configuration);

        document.RootElement
            .GetProperty("Hosting")
            .GetProperty("UseHttpsRedirection")
            .GetBoolean()
            .Should()
            .BeFalse();
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "OutreachFlow.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("The repository root could not be located.");
    }
}
