using System.Linq;
using System.Net;
using System.Text;
using Bunit;
using Bunit.TestDoubles;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OutreachFlow.Web.Attachments;
using OutreachFlow.Web.Campaigns;
using OutreachFlow.Web.Components.Layout;
using OutreachFlow.Web.Components.Pages;
using OutreachFlow.Web.ContactGroups;
using OutreachFlow.Web.ContactImports;
using OutreachFlow.Web.Contacts;
using OutreachFlow.Web.EmailDrafts;
using OutreachFlow.Web.EmailTemplates;
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

        component.Find(".navbar-brand").TextContent.Should().Contain("OutreachFlow");
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

        component.Find(".side-panel-close").Click();

        component.Markup.Should().NotContain("side-panel-body");

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

        Render<Home>().Markup.Should().Contain("Panel");

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

        component.Find("#open-create-sender-profile-panel").Click();
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

        component.Find("#open-create-sender-profile-panel").Click();
        component.Find("#sender-name").Change("Test profile");
        component.Find("#sender-email").Change("invalid-email");
        component.Find("form").Submit();

        component.WaitForAssertion(() =>
        {
            component.Markup.Should().Contain("Enter a valid email address.");
            component.Markup.Should().NotContain("The Email field is not a valid e-mail address.");
        });
    }

    [Fact]
    public void ShouldOpenAndDiscardOrganizationsCreatePanel()
    {
        using var cultureScope = CultureTestScope.Use("es-ES");
        Services.AddLocalization(options => options.ResourcesPath = "Resources");
        Services.AddSingleton(new OrganizationApiClient(CreateHttpClient()));

        var component = Render<Organizations>();

        component.FindAll("#organization-name").Should().BeEmpty();

        component.Find("#open-create-organization-panel").Click();
        component.Markup.Should().Contain("Crear organización");

        component.Find("#organization-name").Change("Acme Corp");
        component.Find(".side-panel-close").Click();

        component.Markup.Should().NotContain("side-panel-body");

        component.Find("#open-create-organization-panel").Click();
        component.Find("#organization-name").GetAttribute("value").Should().BeNullOrEmpty();
    }

    [Fact]
    public void ShouldOpenAndDiscardTagsCreatePanel()
    {
        using var cultureScope = CultureTestScope.Use("es-ES");
        Services.AddLocalization(options => options.ResourcesPath = "Resources");
        Services.AddSingleton(new TagApiClient(CreateHttpClient()));

        var component = Render<Tags>();

        component.Find("#open-create-tag-panel").Click();
        component.Markup.Should().Contain("Crear etiqueta");

        component.Find("#tag-name").Change("VIP");
        component.Find(".side-panel-close").Click();

        component.Markup.Should().NotContain("side-panel-body");
    }

    [Fact]
    public void ShouldOpenAndDiscardAttachmentsUploadPanel()
    {
        using var cultureScope = CultureTestScope.Use("es-ES");
        Services.AddLocalization(options => options.ResourcesPath = "Resources");
        Services.AddSingleton(new AttachmentAssetApiClient(CreateHttpClient()));

        var component = Render<Attachments>();

        component.Find("#open-upload-attachment-panel").Click();
        component.Markup.Should().Contain("Subir adjunto");

        component.Find("#attachment-name").Change("Folleto");
        component.Find(".side-panel-close").Click();

        component.Markup.Should().NotContain("side-panel-body");
    }

    [Fact]
    public void ShouldOpenAndDiscardFollowUpsCreatePanel()
    {
        using var cultureScope = CultureTestScope.Use("es-ES");
        Services.AddLocalization(options => options.ResourcesPath = "Resources");
        Services.AddSingleton(new ContactApiClient(CreateHttpClient()));
        Services.AddSingleton(new FollowUpTaskApiClient(CreateHttpClient()));

        var component = Render<FollowUps>();

        component.Markup.Should().Contain("Pendientes");

        component.Find("#open-create-followup-panel").Click();
        component.Markup.Should().Contain("Crear tarea de seguimiento");

        component.Find(".side-panel-close").Click();
        component.Markup.Should().NotContain("side-panel-body");
    }

    [Fact]
    public void ShouldSelectContactViaSearchSelectInFollowUpsPanel()
    {
        using var cultureScope = CultureTestScope.Use("es-ES");
        Services.AddLocalization(options => options.ResourcesPath = "Resources");
        Services.AddSingleton(new ContactApiClient(new HttpClient(new SingleContactJsonHandler())
        {
            BaseAddress = new Uri("http://localhost")
        }));
        Services.AddSingleton(new FollowUpTaskApiClient(CreateHttpClient()));

        var component = Render<FollowUps>();

        component.Find("#open-create-followup-panel").Click();
        component.Find("#followup-contact").Input("Marta");

        component.Find(".combo-item").Click();

        component.Find("#followup-contact").GetAttribute("value").Should().Be("Marta Silván (marta@example.com)");
    }

    [Fact]
    public void ShouldOpenAndDiscardContactGroupsCreatePanel()
    {
        using var cultureScope = CultureTestScope.Use("es-ES");
        Services.AddLocalization(options => options.ResourcesPath = "Resources");
        Services.AddSingleton(new ContactGroupApiClient(CreateHttpClient()));
        Services.AddSingleton(new ContactApiClient(CreateHttpClient()));
        Services.AddSingleton(new TagApiClient(CreateHttpClient()));

        var component = Render<ContactGroups>();

        component.Find("#open-create-group-panel").Click();
        component.Markup.Should().Contain("Crear grupo");
        component.Find("button[type='submit']").TextContent.Trim().Should().Be("Crear");
        component.Markup.Should().NotContain("Common.Create");

        component.Find(".side-panel-close").Click();
        component.Markup.Should().NotContain("side-panel-body");
    }

    [Fact]
    public void ShouldDiscardSenderProfilePanelInputOnCancel()
    {
        using var cultureScope = CultureTestScope.Use("es-ES");
        Services.AddLocalization(options => options.ResourcesPath = "Resources");
        Services.AddSingleton(new SenderProfileApiClient(CreateHttpClient()));

        var component = Render<SenderProfiles>();

        component.Find("#open-create-sender-profile-panel").Click();
        component.Find("#sender-name").Change("Perfil de prueba");
        component.Find(".side-panel-close").Click();

        component.Markup.Should().NotContain("side-panel-body");

        component.Find("#open-create-sender-profile-panel").Click();
        component.Find("#sender-name").GetAttribute("value").Should().BeNullOrEmpty();
    }

    [Fact]
    public void ShouldOpenAndDiscardTemplatesCreatePanel()
    {
        using var cultureScope = CultureTestScope.Use("es-ES");
        Services.AddLocalization(options => options.ResourcesPath = "Resources");
        Services.AddSingleton(new EmailTemplateApiClient(CreateHttpClient()));
        Services.AddSingleton(new AttachmentAssetApiClient(CreateHttpClient()));

        var component = Render<Templates>();

        component.Find("#open-create-template-panel").Click();
        component.Markup.Should().Contain("Crear plantilla");

        component.Find("#template-name").Change("Seguimiento inicial");
        component.Find(".side-panel-close").Click();

        component.Markup.Should().NotContain("side-panel-body");

        component.Find("#open-create-template-panel").Click();
        component.Find("#template-name").GetAttribute("value").Should().BeNullOrEmpty();
    }

    [Fact]
    public void ShouldOpenAndDiscardCampaignsCreatePanel()
    {
        using var cultureScope = CultureTestScope.Use("es-ES");
        Services.AddLocalization(options => options.ResourcesPath = "Resources");
        Services.AddSingleton(new CampaignApiClient(CreateHttpClient()));
        Services.AddSingleton(new ContactGroupApiClient(CreateHttpClient()));
        Services.AddSingleton(new EmailTemplateApiClient(CreateHttpClient()));

        var component = Render<Campaigns>();

        component.Find("#open-create-campaign-panel").Click();
        component.Markup.Should().Contain("Crear campaña");

        component.Find("#campaign-name").Change("Campaña de otoño");
        component.Find(".side-panel-close").Click();

        component.Markup.Should().NotContain("side-panel-body");

        component.Find("#open-create-campaign-panel").Click();
        component.Find("#campaign-name").GetAttribute("value").Should().BeNullOrEmpty();
    }

    [Fact]
    public void ShouldShowCampaignAudienceAndToggleStatus()
    {
        using var cultureScope = CultureTestScope.Use("es-ES");
        Services.AddLocalization(options => options.ResourcesPath = "Resources");

        var httpClient = new HttpClient(new SingleCampaignJsonHandler())
        {
            BaseAddress = new Uri("http://localhost")
        };

        Services.AddSingleton(new CampaignApiClient(httpClient));
        Services.AddSingleton(new CampaignRecipientApiClient(httpClient));
        Services.AddSingleton(new ContactGroupApiClient(httpClient));
        Services.AddSingleton(new EmailTemplateApiClient(httpClient));
        Services.AddSingleton(new SenderProfileApiClient(httpClient));

        var component = Render<CampaignDetail>(parameters =>
            parameters.Add(p => p.Id, SingleCampaignJsonHandler.CampaignId));

        component.Markup.Should().Contain("Abierta");
        component.Markup.Should().Contain("Prospectos");
        component.Markup.Should().Contain("Intro");

        component.FindAll("button").First(button => button.TextContent.Trim() == "Cerrar campaña").Click();

        component.Markup.Should().Contain("Cerrada");
        component.FindAll("button").Should().Contain(button => button.TextContent.Trim() == "Reabrir campaña");

        component.FindAll("button").First(button => button.TextContent.Trim() == "Editar").Click();
        component.Find("#campaign-edit-name").GetAttribute("value").Should().Be("Campaña de otoño");
        component.Find("#campaign-edit-audience option[selected]").TextContent.Should().Be("Prospectos");
    }

    [Fact]
    public void ShouldIncorporateCandidateIntoCampaign()
    {
        using var cultureScope = CultureTestScope.Use("es-ES");
        Services.AddLocalization(options => options.ResourcesPath = "Resources");

        var handler = new SingleCampaignJsonHandler();
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost")
        };

        Services.AddSingleton(new CampaignApiClient(httpClient));
        Services.AddSingleton(new CampaignRecipientApiClient(httpClient));
        Services.AddSingleton(new ContactGroupApiClient(httpClient));
        Services.AddSingleton(new EmailTemplateApiClient(httpClient));
        Services.AddSingleton(new SenderProfileApiClient(httpClient));

        var component = Render<CampaignDetail>(parameters =>
            parameters.Add(p => p.Id, SingleCampaignJsonHandler.CampaignId));

        component.Markup.Should().Contain("Jamie Smith");

        component.FindAll("button").First(button => button.TextContent.Trim() == "Incorporar").Click();

        component.Markup.Should().Contain("Ahora mismo no hay contactos nuevos");
        component.Markup.Should().Contain("Incorporado");
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

    private sealed class SingleCampaignJsonHandler : HttpMessageHandler
    {
        public static readonly Guid CampaignId = Guid.Parse("5c9c4a2e-8b1e-4b8a-9a2f-1a2b3c4d5e6f");
        private static readonly Guid TemplateId = Guid.Parse("2b3c4d5e-6f70-4a1b-8c2d-3e4f5a6b7c8d");
        private static readonly Guid GroupId = Guid.Parse("7d8e9f0a-1b2c-4d3e-9f0a-1b2c3d4e5f6a");
        private static readonly Guid CandidateContactId = Guid.Parse("9e0f1a2b-3c4d-4e5f-8a9b-0c1d2e3f4a5b");

        private bool _incorporated;

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var path = request.RequestUri?.AbsolutePath ?? string.Empty;

            if (path.EndsWith("/close", StringComparison.Ordinal))
            {
                return Task.FromResult(CreateJsonResponse(CampaignJson("Closed")));
            }

            if (path.EndsWith("/reopen", StringComparison.Ordinal))
            {
                return Task.FromResult(CreateJsonResponse(CampaignJson("Open")));
            }

            if (request.Method == HttpMethod.Get && path.Contains("/contact-groups", StringComparison.Ordinal))
            {
                return Task.FromResult(CreateJsonResponse($$"""
                    [{"id":"{{GroupId}}","name":"Prospectos","createdAt":"2026-01-01T00:00:00Z","updatedAt":"2026-01-01T00:00:00Z","criteria":[]}]
                    """));
            }

            if (request.Method == HttpMethod.Get && path.Contains("/templates", StringComparison.Ordinal))
            {
                return Task.FromResult(CreateJsonResponse($$"""
                    [{"id":"{{TemplateId}}","name":"Intro","description":null,"subjectTemplate":"Hello","bodyTemplate":"Body","defaultAttachmentIds":[],"isActive":true,"createdAt":"2026-01-01T00:00:00Z","updatedAt":"2026-01-01T00:00:00Z"}]
                    """));
            }

            if (request.Method == HttpMethod.Get && path.Contains("/sender-profiles", StringComparison.Ordinal))
            {
                return Task.FromResult(CreateJsonResponse("[]"));
            }

            if (request.Method == HttpMethod.Post && path.EndsWith($"/recipients/{CandidateContactId}", StringComparison.Ordinal))
            {
                _incorporated = true;
                return Task.FromResult(CreateJsonResponse(RecipientJson()));
            }

            if (request.Method == HttpMethod.Get && path.Contains("/candidates", StringComparison.Ordinal))
            {
                return Task.FromResult(CreateJsonResponse(_incorporated
                    ? "[]"
                    : $$"""
                        [{"contactId":"{{CandidateContactId}}","displayName":"Jamie Smith","email":"jamie@example.com"}]
                        """));
            }

            if (request.Method == HttpMethod.Get && path.Contains("/recipients", StringComparison.Ordinal))
            {
                return Task.FromResult(CreateJsonResponse(_incorporated ? $"[{RecipientJson()}]" : "[]"));
            }

            if (request.Method == HttpMethod.Get && path.Contains("/campaigns", StringComparison.Ordinal))
            {
                return Task.FromResult(CreateJsonResponse(CampaignJson("Open")));
            }

            return Task.FromResult(CreateJsonResponse("{}"));
        }

        private static string CampaignJson(string status) => $$"""
            {"id":"{{CampaignId}}","name":"Campaña de otoño","description":"Alcanzar nuevos prospectos","emailTemplateId":"{{TemplateId}}","status":"{{status}}","audienceGroupIds":["{{GroupId}}"],"createdAt":"2026-01-01T00:00:00Z","updatedAt":"2026-01-01T00:00:00Z"}
            """;

        private static string RecipientJson() => $$"""
            {"id":"1f2e3d4c-5b6a-4978-9a0b-1c2d3e4f5a6b","campaignId":"{{CampaignId}}","contactId":"{{CandidateContactId}}","contactDisplayName":"Jamie Smith","contactEmail":"jamie@example.com","messageTemplateId":"{{TemplateId}}","status":"Incorporated","emailDraftId":null,"exclusionReason":null,"incorporatedAt":"2026-01-01T00:00:00Z","updatedAt":"2026-01-01T00:00:00Z"}
            """;

        private static HttpResponseMessage CreateJsonResponse(string json) =>
            new(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
    }

    private sealed class SingleContactJsonHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (request.Method == HttpMethod.Get)
            {
                const string contactsJson = """
                    [
                        {
                            "id": "3a1f7c2e-6b4d-4e63-9a2f-5f6f1c8d3a11",
                            "organizationId": null,
                            "organizationName": null,
                            "displayName": "Marta Silván",
                            "email": "marta@example.com",
                            "phone": null,
                            "role": null,
                            "source": null,
                            "status": "New",
                            "doNotContact": false,
                            "lastContactedAt": null,
                            "createdAt": "2026-01-01T00:00:00Z",
                            "updatedAt": "2026-01-01T00:00:00Z",
                            "tags": []
                        }
                    ]
                    """;

                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(contactsJson, Encoding.UTF8, "application/json")
                });
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{}", Encoding.UTF8, "application/json")
            });
        }
    }
}
