using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using Bunit;
using Bunit.TestDoubles;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OutreachFlow.Web.Components.Layout;
using OutreachFlow.Web.Components.Pages;
using OutreachFlow.Web.Contacts;
using OutreachFlow.Web.Organizations;
using OutreachFlow.Web.Tags;

namespace OutreachFlow.IntegrationTests.Web;

public sealed class WebLocalizationComponentTests : BunitContext
{
    [Fact]
    public void ShouldRenderNavigationInSpanish()
    {
        using var cultureScope = UseCulture("es-ES");
        JSInterop.Setup<string>("cultureHelper.getCulture").SetResult("es-ES");
        Services.AddLocalization(options => options.ResourcesPath = "Resources");

        var component = Render<NavMenu>();

        component.Markup.Should().Contain("Contactos");
        component.Markup.Should().Contain("Organizaciones");
        component.Markup.Should().Contain("Plantillas");
    }

    [Fact]
    public void ShouldRenderSpanishLanguageSelectorAsSelected()
    {
        using var cultureScope = UseCulture("es-ES");
        JSInterop.Setup<string>("cultureHelper.getCulture").SetResult("es-ES");
        Services.AddLocalization(options => options.ResourcesPath = "Resources");

        var component = Render<NavMenu>();

        component.WaitForAssertion(() =>
            component.Find("#sidebar-language-select").GetAttribute("value").Should().Be("es-ES"));
    }

    [Fact]
    public async Task ShouldPersistLanguageSelectionAndForceReloadCurrentRoute()
    {
        using var cultureScope = UseCulture("en-US");
        JSInterop.Mode = JSRuntimeMode.Strict;
        JSInterop.Setup<string>("cultureHelper.getCulture").SetResult("en-US");
        var setCultureCall = JSInterop.Setup<string>("cultureHelper.setCulture", invocation =>
            invocation.Arguments.Count == 1 &&
            string.Equals(invocation.Arguments[0]?.ToString(), "es-ES", StringComparison.Ordinal))
            .SetResult("es-ES");
        Services.AddLocalization(options => options.ResourcesPath = "Resources");

        var nav = Services.GetRequiredService<BunitNavigationManager>();
        var currentUri = nav.Uri;
        var component = Render<NavMenu>();

        await component.InvokeAsync(() => component.Find("#sidebar-language-select").Change("es-ES"));

        component.WaitForAssertion(() =>
        {
            setCultureCall.Invocations.Should().ContainSingle();
            nav.Uri.Should().Be(currentUri);
            nav.History.Should().NotBeEmpty();
            nav.History.First().Uri.Should().Be(currentUri);
            nav.History.First().Options.ForceLoad.Should().BeTrue();
        });
    }

    [Fact]
    public void ShouldRenderContactsPageLabelsInSpanish()
    {
        using var cultureScope = UseCulture("es-ES");
        JSInterop.Setup<string>("cultureHelper.getCulture").SetResult("es-ES");
        Services.AddLocalization(options => options.ResourcesPath = "Resources");

        var httpClient = new HttpClient(new EmptyArrayJsonHandler())
        {
            BaseAddress = new Uri("http://localhost")
        };

        Services.AddSingleton(new ContactApiClient(httpClient));
        Services.AddSingleton(new OrganizationApiClient(httpClient));
        Services.AddSingleton(new TagApiClient(httpClient));

        var component = Render<Contacts>();

        component.Markup.Should().Contain("Contactos");
        component.Markup.Should().Contain("Filtros");
        component.Markup.Should().Contain("Crear contacto");
    }

    private static CultureScope UseCulture(string culture)
    {
        var originalCulture = CultureInfo.CurrentCulture;
        var originalUiCulture = CultureInfo.CurrentUICulture;
        var nextCulture = new CultureInfo(culture);

        CultureInfo.CurrentCulture = nextCulture;
        CultureInfo.CurrentUICulture = nextCulture;

        return new CultureScope(originalCulture, originalUiCulture);
    }

    private sealed class CultureScope(CultureInfo culture, CultureInfo uiCulture) : IDisposable
    {
        public void Dispose()
        {
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = uiCulture;
        }
    }

    private sealed class EmptyArrayJsonHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (request.Method == HttpMethod.Get)
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("[]", Encoding.UTF8, "application/json")
                });
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{}", Encoding.UTF8, "application/json")
            });
        }
    }
}
