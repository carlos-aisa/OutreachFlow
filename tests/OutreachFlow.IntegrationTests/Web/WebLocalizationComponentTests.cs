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
using OutreachFlow.Web.SenderProfiles;
using OutreachFlow.Web.Tags;

namespace OutreachFlow.IntegrationTests.Web;

[Collection(CultureSensitiveTestCollectionDefinition.Name)]
public sealed class WebLocalizationComponentTests : BunitContext
{
    [Fact]
    public void ShouldRenderNavigationInSpanish()
    {
        using var cultureScope = CultureTestScope.Use("es-ES");
        Services.AddLocalization(options => options.ResourcesPath = "Resources");

        var component = Render<NavMenu>();

        component.Markup.Should().Contain("Contactos");
        component.Markup.Should().Contain("Organizaciones");
        component.Markup.Should().Contain("Plantillas");
        component.Markup.Should().Contain("Espacio de trabajo");
        component.Markup.Should().Contain("Configuración");
    }

    [Fact]
    public void ShouldRenderSettingsPageInSpanish()
    {
        using var cultureScope = CultureTestScope.Use("es-ES");
        JSInterop.Setup<string>("cultureHelper.getCulture").SetResult("es-ES");
        JSInterop.Setup<string>("themeHelper.getTheme").SetResult("dark");
        Services.AddLocalization(options => options.ResourcesPath = "Resources");

        var component = Render<Settings>();

        component.Markup.Should().Contain("Configuración");
        component.Markup.Should().Contain("Preferencias generales");
        component.Markup.Should().Contain("Idioma");
        component.Markup.Should().Contain("Tema");
        component.Markup.Should().Contain("Oscuro");
    }

    [Fact]
    public void ShouldRenderPersistedLanguageAndThemeSelectionsInSettings()
    {
        using var cultureScope = CultureTestScope.Use("es-ES");
        JSInterop.Setup<string>("cultureHelper.getCulture").SetResult("es-ES");
        JSInterop.Setup<string>("themeHelper.getTheme").SetResult("dark");
        Services.AddLocalization(options => options.ResourcesPath = "Resources");

        var component = Render<Settings>();

        component.WaitForAssertion(() =>
        {
            component.Find("#settings-language-select").GetAttribute("value").Should().Be("es-ES");
            component.Find("#settings-theme-select").GetAttribute("value").Should().Be("dark");
        });
    }

    [Fact]
    public void ShouldRenderSidebarBrandAndSettingsDestinationInsideNavigationShell()
    {
        using var cultureScope = CultureTestScope.Use("en-US");
        JSInterop.Setup<string>("cultureHelper.getCulture").SetResult("en-US");
        Services.AddLocalization(options => options.ResourcesPath = "Resources");

        var component = Render<NavMenu>();

        component.Find(".sidebar-brand-panel").TextContent.Should().Contain("OutreachFlow");
        component.Find("a[href='settings']").TextContent.Should().Contain("Settings");
        component.FindAll("#sidebar-language-select").Should().BeEmpty();
    }

    [Fact]
    public void ShouldMarkCurrentNavigationItemAsActive()
    {
        using var cultureScope = CultureTestScope.Use("en-US");
        Services.AddLocalization(options => options.ResourcesPath = "Resources");

        var nav = Services.GetRequiredService<BunitNavigationManager>();
        nav.NavigateTo("http://localhost/settings");

        var component = Render<NavMenu>();

        component.Find("a[href='settings']").ClassList.Should().Contain("active");
    }

    [Fact]
    public async Task ShouldPersistLanguageSelectionAndForceReloadCurrentRoute()
    {
        using var cultureScope = CultureTestScope.Use("en-US");
        JSInterop.Mode = JSRuntimeMode.Strict;
        JSInterop.Setup<string>("cultureHelper.getCulture").SetResult("en-US");
        JSInterop.Setup<string>("themeHelper.getTheme").SetResult("system");
        var setCultureCall = JSInterop.Setup<string>("cultureHelper.setCulture", invocation =>
            invocation.Arguments.Count == 1 &&
            string.Equals(invocation.Arguments[0]?.ToString(), "es-ES", StringComparison.Ordinal))
            .SetResult("es-ES");
        Services.AddLocalization(options => options.ResourcesPath = "Resources");

        var nav = Services.GetRequiredService<BunitNavigationManager>();
        var currentUri = nav.Uri;
        var component = Render<Settings>();

        await component.InvokeAsync(() => component.Find("#settings-language-select").Change("es-ES"));

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
    public async Task ShouldPersistThemeSelectionWithoutReloadingCurrentRoute()
    {
        using var cultureScope = CultureTestScope.Use("en-US");
        JSInterop.Mode = JSRuntimeMode.Strict;
        JSInterop.Setup<string>("cultureHelper.getCulture").SetResult("en-US");
        JSInterop.Setup<string>("themeHelper.getTheme").SetResult("system");
        var setThemeCall = JSInterop.Setup<string>("themeHelper.setTheme", invocation =>
            invocation.Arguments.Count == 1 &&
            string.Equals(invocation.Arguments[0]?.ToString(), "dark", StringComparison.Ordinal))
            .SetResult("dark");
        Services.AddLocalization(options => options.ResourcesPath = "Resources");

        var nav = Services.GetRequiredService<BunitNavigationManager>();
        var currentUri = nav.Uri;
        var component = Render<Settings>();

        await component.InvokeAsync(() => component.Find("#settings-theme-select").Change("dark"));

        component.WaitForAssertion(() =>
        {
            setThemeCall.Invocations.Should().ContainSingle();
            nav.Uri.Should().Be(currentUri);
            nav.History.Should().BeEmpty();
            component.Find("#settings-theme-select").GetAttribute("value").Should().Be("dark");
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

        component.Find("#open-create-contact-panel").Click();

        component.Markup.Should().Contain("Crear contacto");
        component.Markup.Should().Contain("Empieza por la persona");

        component.Find("#new-contact-organization").Input("Nueva Organización S.L.");
        component.Find(".combo-item--create").Click();

        component.Markup.Should().Contain("Nueva organización");

        component.Find(".org-extra-toggle").Click();

        component.Markup.Should().Contain("Datos de la nueva organizaci");
    }

    [Fact]
    public void ShouldSelectExistingOrganizationAndDiscardPanelInputOnCancel()
    {
        using var cultureScope = CultureTestScope.Use("es-ES");
        JSInterop.Setup<string>("cultureHelper.getCulture").SetResult("es-ES");
        Services.AddLocalization(options => options.ResourcesPath = "Resources");

        var httpClient = new HttpClient(new SingleOrganizationJsonHandler())
        {
            BaseAddress = new Uri("http://localhost")
        };

        Services.AddSingleton(new ContactApiClient(httpClient));
        Services.AddSingleton(new OrganizationApiClient(httpClient));
        Services.AddSingleton(new TagApiClient(httpClient));

        var component = Render<Contacts>();

        component.Find("#open-create-contact-panel").Click();
        component.Find("#new-contact-organization").Input("Acme");

        component.Find(".combo-item").Click();

        component.Find("#new-contact-organization").GetAttribute("value").Should().Be("Acme Corp");
        component.Markup.Should().NotContain("Nueva organización");

        component.Find(".contact-panel-close").Click();

        component.Markup.Should().NotContain("contact-panel-body");

        component.Find("#open-create-contact-panel").Click();

        component.Find("#new-contact-organization").GetAttribute("value").Should().BeNullOrEmpty();
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

    [Fact]
    public void ShouldRenderSenderProfileValidationMessagesInSpanish()
    {
        using var cultureScope = CultureTestScope.Use("es-ES");
        Services.AddLocalization(options => options.ResourcesPath = "Resources");
        Services.AddSingleton(new SenderProfileApiClient(CreateHttpClient()));

        var component = Render<SenderProfiles>();

        component.Find("#sender-name").Change("Perfil de prueba");
        component.Find("#sender-email").Change("correo-invalido");
        component.Find("form").Submit();

        component.WaitForAssertion(() =>
        {
            component.Markup.Should().Contain("Introduce una dirección de correo válida.");
            component.Markup.Should().NotContain("The Email field is not a valid e-mail address.");
        });
    }

    [Fact]
    public void ShouldRenderSenderProfileValidationMessagesInEnglish()
    {
        using var cultureScope = CultureTestScope.Use("en-US");
        Services.AddLocalization(options => options.ResourcesPath = "Resources");
        Services.AddSingleton(new SenderProfileApiClient(CreateHttpClient()));

        var component = Render<SenderProfiles>();

        component.Find("#sender-name").Change("Test profile");
        component.Find("#sender-email").Change("invalid-email");
        component.Find("form").Submit();

        component.WaitForAssertion(() =>
        {
            component.Markup.Should().Contain("Enter a valid email address.");
            component.Markup.Should().NotContain("The Email field is not a valid e-mail address.");
        });
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

    private sealed class SingleOrganizationJsonHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (request.Method == HttpMethod.Get && request.RequestUri is not null &&
                request.RequestUri.AbsolutePath.Contains("organizations", StringComparison.OrdinalIgnoreCase))
            {
                const string organizationsJson = """
                    [
                        {
                            "id": "9c3f7e2a-4b7d-4e63-9a2f-5f6f1c8d3a10",
                            "name": "Acme Corp",
                            "type": null,
                            "website": null,
                            "city": null,
                            "province": null,
                            "country": null,
                            "notes": null,
                            "createdAt": "2026-01-01T00:00:00Z",
                            "updatedAt": "2026-01-01T00:00:00Z"
                        }
                    ]
                    """;

                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(organizationsJson, Encoding.UTF8, "application/json")
                });
            }

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
