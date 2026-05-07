using System.Text.Json.Serialization;
using System.Globalization;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using OutreachFlow.Api.Endpoints;
using OutreachFlow.Api.Errors;
using OutreachFlow.Application.DependencyInjection;
using OutreachFlow.Infrastructure.DependencyInjection;
using OutreachFlow.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddWindowsService();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IApiErrorLocalizer, ApiErrorLocalizer>();
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
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
ApiEndpoint.ConfigureHttpContextAccessor(app.Services.GetRequiredService<IHttpContextAccessor>());
app.UseRequestLocalization();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<OutreachFlowDbContext>();
    await dbContext.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var useHttpsRedirection = builder.Configuration.GetValue("Hosting:UseHttpsRedirection", true);
if (!app.Environment.IsEnvironment("Testing") && useHttpsRedirection)
{
    app.UseHttpsRedirection();
}

app.MapGet("/api/v1/health", () =>
    Results.Ok(new
    {
        status = "Healthy",
        utcNow = DateTimeOffset.UtcNow
    }))
    .WithName("GetHealth")
    .WithOpenApi();

app.MapOrganizationEndpoints();
app.MapContactEndpoints();
app.MapTagEndpoints();
app.MapSenderProfileEndpoints();
app.MapEmailTemplateEndpoints();
app.MapAttachmentAssetEndpoints();
app.MapEmailDraftEndpoints();
app.MapFollowUpTaskEndpoints();
app.MapContactImportEndpoints();

app.Run();

public partial class Program;
