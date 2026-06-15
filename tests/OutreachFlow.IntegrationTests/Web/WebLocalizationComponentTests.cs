using System.Linq;
using System.Net;
using System.Text;
using Bunit;
using Bunit.TestDoubles;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OutreachFlow.Web.Attachments;
using OutreachFlow.Web.Components.Layout;
using OutreachFlow.Web.Components.Pages;
using OutreachFlow.Web.ContactImports;
using OutreachFlow.Web.Contacts;
using OutreachFlow.Web.EmailDrafts;
using OutreachFlow.Web.FollowUps;
using OutreachFlow.Web.Organizations;
using OutreachFlow.Web.Tags;

namespace OutreachFlow.IntegrationTests.Web;

[Collection(CultureSensitiveTestCollectionDefinition.Name)]
public sealed class WebLocalizationComponentTests : BunitContext
{
    [Fact]
    public void ShouldRenderNavigationInSpanish()
    {
        using var cultureScope = CultureTestScope.Use("es-ES");
        JSInterop.Setup<string>("cultureHelper.getCulture").SetResult("es-ES");
        Services.AddLocalization(options => options.ResourcesPath = "Resources");

        var component = Render<NavMenu>();

        component.Markup.Should().Contain("Contactos");
        component.Markup.Should().Contain("Organizaciones");
        component.Markup.Should().Contain("Plantillas");
        component.Markup.Should().Contain("Espacio de trabajo");
        component.Markup.Should().Contain("Idioma");
    }

    [Fact]
    public void ShouldRenderSpanishLanguageSelectorAsSelected()
    {
        using var cultureScope = CultureTestScope.Use("es-ES");
        JSInterop.Setup<string>("cultureHelper.getCulture").SetResult("es-ES");
        Services.AddLocalization(options => options.ResourcesPath = "Resources");

        var component = Render<NavMenu>();

        component.WaitForAssertion(() =>
            component.Find("#sidebar-language-select").GetAttribute("value").Should().Be("es-ES"));
    }

    [Fact]
    public void ShouldRenderSidebarBrandAndLanguageSelectorInsideNavigationShell()
    {
        using var cultureScope = CultureTestScope.Use("en-US");
        JSInterop.Setup<string>("cultureHelper.getCulture").SetResult("en-US");
        Services.AddLocalization(options => options.ResourcesPath = "Resources");

        var component = Render<NavMenu>();

        component.Find(".sidebar-brand-panel").TextContent.Should().Contain("OutreachFlow");
        component.Find(".nav-control-panel #sidebar-language-select").Should().NotBeNull();
    }

    [Fact]
    public void ShouldMarkCurrentNavigationItemAsActive()
    {
        using var cultureScope = CultureTestScope.Use("en-US");
        JSInterop.Setup<string>("cultureHelper.getCulture").SetResult("en-US");
        Services.AddLocalization(options => options.ResourcesPath = "Resources");

        var nav = Services.GetRequiredService<BunitNavigationManager>();
        nav.NavigateTo("http://localhost/contacts");

        var component = Render<NavMenu>();

        component.Find("a[href='contacts']").ClassList.Should().Contain("active");
    }

    [Fact]
    public async Task ShouldPersistLanguageSelectionAndForceReloadCurrentRoute()
    {
        using var cultureScope = CultureTestScope.Use("en-US");
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
        using var cultureScope = CultureTestScope.Use("es-ES");
        JSInterop.Setup<string>("cultureHelper.getCulture").SetResult("es-ES");
        Services.AddLocalization(options => options.ResourcesPath = "Resources");

        var httpClient = CreateHttpClient();

        Services.AddSingleton(new ContactApiClient(httpClient));
        Services.AddSingleton(new OrganizationApiClient(httpClient));
        Services.AddSingleton(new TagApiClient(httpClient));

        var component = Render<Contacts>();

        component.Markup.Should().Contain("Contactos");
        component.Markup.Should().Contain("Filtros");
        component.Markup.Should().Contain("Crear contacto");
    }

    [Fact]
    public void ShouldRenderWorkflowPagesInSpanish()
    {
        using var cultureScope = CultureTestScope.Use("es-ES");
        JSInterop.Setup<string>("cultureHelper.getCulture").SetResult("es-ES");
        Services.AddLocalization(options => options.ResourcesPath = "Resources");

        var httpClient = CreateHttpClient();

        Services.AddSingleton(new ContactApiClient(httpClient));
        Services.AddSingleton(new OrganizationApiClient(httpClient));
        Services.AddSingleton(new TagApiClient(httpClient));
        Services.AddSingleton(new EmailDraftApiClient(httpClient));
        Services.AddSingleton(new FollowUpTaskApiClient(httpClient));
        Services.AddSingleton(new AttachmentAssetApiClient(httpClient));
        Services.AddSingleton(new ContactImportApiClient(httpClient));

        Render<Home>().Markup.Should().Contain("Resumen");

        var draftsMarkup = Render<Drafts>().Markup;
        draftsMarkup.Should().Contain("Borradores");
        draftsMarkup.Should().Contain("Cualquiera");
        draftsMarkup.Should().Contain("Aprobado");

        Render<FollowUps>().Markup.Should().Contain("Seguimientos");
        Render<Attachments>().Markup.Should().Contain("Adjuntos");
        Render<Imports>().Markup.Should().Contain("Importaciones");

        var errorMarkup = Render<Error>().Markup;
        errorMarkup.Should().Contain("Modo de desarrollo");
        errorMarkup.Should().Contain("Se produjo un error al procesar tu solicitud.");
    }

    private static HttpClient CreateHttpClient()
    {
        return new HttpClient(new EmptyArrayJsonHandler())
        {
            BaseAddress = new Uri("http://localhost")
        };
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
