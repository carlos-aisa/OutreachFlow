using OutreachFlow.Web.Components;
using OutreachFlow.Web.Contacts;
using OutreachFlow.Web.EmailDrafts;
using OutreachFlow.Web.Attachments;
using OutreachFlow.Web.EmailTemplates;
using OutreachFlow.Web.Organizations;
using OutreachFlow.Web.SenderProfiles;
using OutreachFlow.Web.Tags;
using OutreachFlow.Web.FollowUps;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
