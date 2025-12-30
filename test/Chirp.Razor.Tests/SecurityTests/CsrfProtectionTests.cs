using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Chirp.Razor.Tests.SecurityTests;

public class CsrfProtectionTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public CsrfProtectionTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task PostCheep_WithoutAntiForgeryToken_ReturnsBadRequest()
    {
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("Text", "Test cheep without csrf token")
        });

        var response = await _client.PostAsync("/", content);

        // so without csrf token, we should return 400 (bad request)
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostCheep_WithInvalidAntiForgeryToken_ReturnsBadRequest()
    {
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("Text", "Test cheep"),
            new KeyValuePair<string, string>("__RequestVerificationToken", "invalid-token-12345")
        });

        var response = await _client.PostAsync("/", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task LikeApi_WithoutAntiForgeryToken_ReturnsBadRequestOrUnauthorized()
    {
        var response = await _client.PostAsync("/api/likes/1", null);

        // here should return either 400 (missing csrf) or 401 (not authenticated)
        Assert.True(
            response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.Unauthorized,
            $"Expected 400 or 401, got {response.StatusCode}");
    }

    [Fact]
    public async Task LikeApi_WithInvalidAntiForgeryToken_ReturnsBadRequestOrUnauthorized()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/likes/1");
        request.Headers.Add("RequestVerificationToken", "invalid-token");

        var response = await _client.SendAsync(request);

        Assert.True(
            response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.Unauthorized,
            $"Expected 400 or 401, got {response.StatusCode}");
    }

    [Fact]
    public async Task FollowForm_WithoutAntiForgeryToken_ReturnsBadRequest()
    {
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("targetName", "SomeUser"),
            new KeyValuePair<string, string>("handler", "Follow")
        });

        var response = await _client.PostAsync("/?handler=Follow", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
