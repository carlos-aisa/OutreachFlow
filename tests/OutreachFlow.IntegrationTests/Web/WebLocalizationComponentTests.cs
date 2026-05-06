using System.Globalization;
using System.Net;
using System.Text;
using Bunit;
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
        Services.AddLocalization(options => options.ResourcesPath = "Resources");

        var component = Render<NavMenu>();

        component.Markup.Should().Contain("Contactos");
        component.Markup.Should().Contain("Organizaciones");
        component.Markup.Should().Contain("Plantillas");
    }

    [Fact]
    public void ShouldRenderContactsPageLabelsInSpanish()
    {
        using var cultureScope = UseCulture("es-ES");
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
