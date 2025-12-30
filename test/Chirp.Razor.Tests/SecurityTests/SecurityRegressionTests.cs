using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Chirp.Razor.Tests.SecurityTests;

public class SecurityRegressionTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly string _srcPath;

    public SecurityRegressionTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        // we navigate from test bin dir to src dir
        _srcPath = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory, "..", "..", "..", "..", "..", "src"));
    }

    [Fact]
    public void NoHtmlRaw_InViewFiles_WithUserContent()
    {
        var viewsPath = Path.Combine(_srcPath, "Chirp.Web", "Pages");
        var cshtmlFiles = Directory.GetFiles(viewsPath, "*.cshtml", SearchOption.AllDirectories);

        var violations = new List<string>();

        foreach (var file in cshtmlFiles)
        {
            var content = File.ReadAllText(file);
            var lines = content.Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                // checking for Html.Raw with dynamic content (and NOT static strings)
                if (line.Contains("Html.Raw(") && !line.Contains("Html.Raw(\""))
                {
                    violations.Add($"{Path.GetFileName(file)}:{i + 1} - {line.Trim()}");
                }
            }
        }

        Assert.True(violations.Count == 0,
            $"found Html.Raw() with dynamic content (and it might cause XSS):\n{string.Join("\n", violations)}");
    }

    [Fact]
    public void NoRawSql_InRepositories()
    {
        var repoPath = Path.Combine(_srcPath, "Chirp.Infrastructure", "Chirp.Repositories");
        var csFiles = Directory.GetFiles(repoPath, "*.cs", SearchOption.AllDirectories);

        var dangerousPatterns = new[]
        {
            "FromSqlRaw",
            "ExecuteSqlRaw",
            "SqlQuery<",
            "Database.ExecuteSql"
        };

        var violations = new List<string>();

        foreach (var file in csFiles)
        {
            var content = File.ReadAllText(file);
            var lines = content.Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                foreach (var pattern in dangerousPatterns)
                {
                    if (line.Contains(pattern))
                    {
                        violations.Add($"{Path.GetFileName(file)}:{i + 1} - found '{pattern}'");
                    }
                }
            }
        }

        Assert.True(violations.Count == 0,
            $"found raw sql methods (and which might cause SQL injection):\n{string.Join("\n", violations)}");
    }

    [Fact]
    public void AllPostForms_HaveAntiForgeryTokens()
    {
        var viewsPath = Path.Combine(_srcPath, "Chirp.Web", "Pages");
        var cshtmlFiles = Directory.GetFiles(viewsPath, "*.cshtml", SearchOption.AllDirectories);

        var violations = new List<string>();

        foreach (var file in cshtmlFiles)
        {
            var content = File.ReadAllText(file);

            // finding all POST forms
            var formMatches = System.Text.RegularExpressions.Regex.Matches(
                content,
                @"<form[^>]*method\s*=\s*[""']post[""'][^>]*>",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            foreach (System.Text.RegularExpressions.Match match in formMatches)
            {
                // finding the closing </form> tag
                var formStart = match.Index;
                var formEnd = content.IndexOf("</form>", formStart);

                if (formEnd > formStart)
                {
                    var formContent = content.Substring(formStart, formEnd - formStart);
                    var formTag = match.Value;

                    // checking if the form has anti forgery token (either directly or through tag helpers)
                    // ASP.NET tag helpers (asp page, asp page handler, asp action) ahs to automatically add csrf tokens
                    var hasExplicitToken = formContent.Contains("AntiForgeryToken") ||
                                           formContent.Contains("asp-antiforgery") ||
                                           formContent.Contains("__RequestVerificationToken");

                    var hasTagHelper = formTag.Contains("asp-page") ||
                                       formTag.Contains("asp-action") ||
                                       formTag.Contains("asp-controller") ||
                                       formTag.Contains("asp-route");

                    if (!hasExplicitToken && !hasTagHelper)
                    {
                        // getting the line number
                        var lineNumber = content.Substring(0, formStart).Count(c => c == '\n') + 1;
                        violations.Add($"{Path.GetFileName(file)}:{lineNumber} - POST form without CSRF token");
                    }
                }
            }
        }

        Assert.True(violations.Count == 0,
            $"Found POST forms without anti-forgery tokens:\n{string.Join("\n", violations)}");
    }

    [Fact]
    public async Task SecurityHeaders_ArePresent()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/");

        // checking the different headers:

        // content-security-oolicy header
        Assert.True(response.Headers.Contains("Content-Security-Policy"),
            "Content-Security-Policy header is missing");

        // x-content-type-options header
        Assert.True(response.Headers.Contains("X-Content-Type-Options"),
            "X-Content-Type-Options header is missing");

        // x-frame-options header
        Assert.True(response.Headers.Contains("X-Frame-Options"),
            "X-Frame-Options header is missing");

        // checking and veryfying the header values
        var csp = response.Headers.GetValues("Content-Security-Policy").First();
        Assert.Contains("default-src", csp);

        var xcto = response.Headers.GetValues("X-Content-Type-Options").First();
        Assert.Equal("nosniff", xcto);

        var xfo = response.Headers.GetValues("X-Frame-Options").First();
        Assert.Equal("DENY", xfo);
    }
}
