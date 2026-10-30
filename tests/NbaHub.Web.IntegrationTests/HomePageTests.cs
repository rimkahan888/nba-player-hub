using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace NbaHub.Web.IntegrationTests;

public class HomePageTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Home_page_renders_blazor_app_shell()
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync(new Uri("/", UriKind.Relative));
        var html = await response.Content.ReadAsStringAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("text/html");
        html.ShouldContain("<h1>NBA Player Hub</h1>");
        html.ShouldContain("_framework/blazor.web");
    }

    [Fact]
    public async Task Unknown_route_returns_not_found_page()
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync(new Uri("/no-such-page", UriKind.Relative));
        var html = await response.Content.ReadAsStringAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        html.ShouldContain("Not Found");
    }
}
