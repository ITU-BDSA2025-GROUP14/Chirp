using System.Text;
using System.Text.Json;

namespace Chirp.CLI.Client;

public class HttpDatabaseRepository : IDatabaseRepository<Cheep>
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public HttpDatabaseRepository(string baseUrl = "http://localhost:5000")
    {
        _httpClient = new HttpClient();
        _baseUrl = baseUrl;
    }

    public IEnumerable<Cheep> Read(int limit)
    {
        try
        {
            var response = _httpClient.GetAsync($"{_baseUrl}/cheeps").Result;
            if (response.IsSuccessStatusCode)
            {
                var json = response.Content.ReadAsStringAsync().Result;
                var cheeps = JsonSerializer.Deserialize<List<Cheep>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return cheeps?.Take(limit) ?? Enumerable.Empty<Cheep>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading cheeps: {ex.Message}");
        }
        return Enumerable.Empty<Cheep>();
    }

    public void Store(Cheep record)
    {
        try
        {
            var json = JsonSerializer.Serialize(record);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = _httpClient.PostAsync($"{_baseUrl}/cheep", content).Result;

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error storing cheep: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error storing cheep: {ex.Message}");
        }
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}