using System.Globalization;

namespace OutreachFlow.IntegrationTests.Web;

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class CultureSensitiveTestCollectionDefinition
{
    public const string Name = "Culture sensitive";
}

internal static class CultureTestScope
{
    public static IDisposable Use(string culture)
    {
        var originalCulture = CultureInfo.CurrentCulture;
        var originalUiCulture = CultureInfo.CurrentUICulture;
        var nextCulture = new CultureInfo(culture);

        CultureInfo.CurrentCulture = nextCulture;
        CultureInfo.CurrentUICulture = nextCulture;

        return new Scope(originalCulture, originalUiCulture);
    }

    private sealed class Scope(CultureInfo culture, CultureInfo uiCulture) : IDisposable
    {
        public void Dispose()
        {
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = uiCulture;
        }
    }
}
