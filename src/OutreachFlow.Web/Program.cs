using System.Globalization;
using Microsoft.AspNetCore.Localization;
using OutreachFlow.Web.Components;
using OutreachFlow.Web.Contacts;
using OutreachFlow.Web.EmailDrafts;
using OutreachFlow.Web.Attachments;
using OutreachFlow.Web.EmailTemplates;
using OutreachFlow.Web.Organizations;
using OutreachFlow.Web.SenderProfiles;
using OutreachFlow.Web.Tags;
using OutreachFlow.Web.FollowUps;
using OutreachFlow.Web.ContactImports;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddWindowsService();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en-US"),
        new CultureInfo("es-ES")
    };

    options.DefaultRequestCulture = new RequestCulture("en-US");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    options.RequestCultureProviders =
    [
        new QueryStringRequestCultureProvider(),
        new CookieRequestCultureProvider(),
        new AcceptLanguageHeaderRequestCultureProvider()
    ];
});

var apiBaseUrl = builder.Configuration["OutreachFlowApi:BaseUrl"] ??
    throw new InvalidOperationException("OutreachFlow API base URL is not configured.");

builder.Services.AddHttpClient<ContactApiClient>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<OrganizationApiClient>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<TagApiClient>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<SenderProfileApiClient>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<EmailTemplateApiClient>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<AttachmentAssetApiClient>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<EmailDraftApiClient>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<FollowUpTaskApiClient>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<ContactImportApiClient>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);

    var useHsts = builder.Configuration.GetValue("Hosting:UseHsts", true);
    if (useHsts)
    {
        app.UseHsts();
    }
}

var useHttpsRedirection = builder.Configuration.GetValue("Hosting:UseHttpsRedirection", true);
if (useHttpsRedirection)
{
    app.UseHttpsRedirection();
}
app.UseRequestLocalization();

app.UseStaticFiles();
app.UseAntiforgery();
app.MapGet("/culture/set", (string culture, string? redirectUri, HttpContext context) =>
{
    var safeRedirect = string.IsNullOrWhiteSpace(redirectUri) ? "/" : redirectUri;
    if (!Uri.IsWellFormedUriString(safeRedirect, UriKind.Relative))
    {
        safeRedirect = "/";
    }

    context.Response.Cookies.Append(
        CookieRequestCultureProvider.DefaultCookieName,
        CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
        new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddYears(1),
            IsEssential = true
        });

    return Results.LocalRedirect(safeRedirect);
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
