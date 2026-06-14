using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Testing;

namespace OutreachFlow.IntegrationTests.Web;

public sealed class WebLanguageSelectionTests
{
    [Fact]
    public async Task ShouldSetRootScopedCultureCookieAndRedirectToRequestedPage()
    {
        using var factory = new OutreachFlowWebFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        using var response = await client.GetAsync("/culture/set?culture=es-ES&redirectUri=/contacts");

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.OriginalString.Should().Be("/contacts");
        response.Headers.TryGetValues("Set-Cookie", out var cookieValues).Should().BeTrue();
        cookieValues.Should().NotBeNull();
        cookieValues!.Should().ContainSingle(value =>
            value.Contains($"{CookieRequestCultureProvider.DefaultCookieName}=", StringComparison.Ordinal) &&
            value.Contains("Path=/", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task ShouldRenderSpanishNavigationWhenCultureCookieIsSelectedAndReusedLater()
    {
        string cultureCookie;

        using (var initialFactory = new OutreachFlowWebFactory())
        using (var initialClient = initialFactory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        }))
        {
            using var response = await initialClient.GetAsync("/culture/set?culture=es-ES&redirectUri=/");

            response.Headers.TryGetValues("Set-Cookie", out var cookieValues).Should().BeTrue();
            var setCookieHeader = cookieValues!
                .Single(value => value.Contains(
                    $"{CookieRequestCultureProvider.DefaultCookieName}=",
                    StringComparison.Ordinal));
            cultureCookie = setCookieHeader.Split(';', 2)[0];
        }

        using var laterFactory = new OutreachFlowWebFactory();
        using var laterClient = laterFactory.CreateClient();
        laterClient.DefaultRequestHeaders.Add("Cookie", cultureCookie);

        var html = await laterClient.GetStringAsync("/");

        html.Should().Contain("Contactos");
        html.Should().Contain("Organizaciones");
        html.Should().Contain("Plantillas");
    }
}
