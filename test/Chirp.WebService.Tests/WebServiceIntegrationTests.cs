using Microsoft.AspNetCore.Mvc.Testing;
using System.Text;
using System.Text.Json;
using Chirp.WebService;

namespace Chirp.WebService.Tests;

public class WebServiceIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public WebServiceIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetCheeps_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/cheeps");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task GetCheeps_ReturnsListOfCheeps()
    {
        // Act
        var response = await _client.GetAsync("/cheeps");
        var content = await response.Content.ReadAsStringAsync();
        var cheeps = JsonSerializer.Deserialize<List<Cheep>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.NotNull(cheeps);
        Assert.IsType<List<Cheep>>(cheeps);
    }

    [Fact]
    public async Task PostCheep_ReturnsSuccessStatusCode()
    {
        // Arrange
        var cheep = new Cheep("TestUser", "Test message", DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        var json = JsonSerializer.Serialize(cheep);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/cheep", content);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task PostCheep_ThenGetCheeps_ContainsNewCheep()
    {
        // Arrange
        var testMessage = $"Integration test message {Guid.NewGuid()}";
        var cheep = new Cheep("IntegrationTest", testMessage, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        var json = JsonSerializer.Serialize(cheep);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        await _client.PostAsync("/cheep", content);
        var getResponse = await _client.GetAsync("/cheeps");
        var getCheepsContent = await getResponse.Content.ReadAsStringAsync();
        var allCheeps = JsonSerializer.Deserialize<List<Cheep>>(getCheepsContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.NotNull(allCheeps);
        Assert.Contains(allCheeps, c => c.Message == testMessage && c.Author == "IntegrationTest");
    }
}