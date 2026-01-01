# Chirp!

This is Chirp!, which is a social media web application created for the autumn 2025 ITU BDSA course.

## Features:

- Post messages (cheeps)
- Follow and unfollow other users
- Like cheeps
- View public timeline or specific user timeline
- GitHub OAuth authentication

## Tech stack:

- .NET 8
- ASP.NET Core razorpages
- Entity framework core with SQLite
- ASP.NET identity with GitHub OAuth
- xUnit and Playwright for testing

## Get started:

### Prerequisities:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- GitHub OAuth credentials (which is for for authentication)

### Setting it up:

1. Clone the repo
2. Configure GitHub OAuth ClientId and ClientSecret:
   ```bash
   cd src/Chirp.Web
   dotnet user-secrets init
   dotnet user-secrets set "Authentication:GitHub:ClientId" "<YOUR_CLIENTID>"
   dotnet user-secrets set "Authentication:GitHub:ClientSecret" "<YOUR_CLIENTSECRET>"
   ```

3. To run the application, do:
   ```bash
   dotnet build
   dotnet run --project src/Chirp.Web
   ```

4. Then open http://localhost:5273 in your browser

## Development

```bash
# Build:
dotnet build

# Running all of the tests:
dotnet test

# Running unit/integration tests only (which is faster):
dotnet test test/Chirp.Razor.Tests

# Running E2E tests (this requires Playwright browsers):
dotnet test test/Chirp.PlaywrightTests
```

## Project Structure

```
src/
  Chirp.Core/                # domain entities and interfaces
  Chirp.Infrastructure/      # EF Core, repos, services
  Chirp.Web/                 # razorpages web application
test/                        # tests:
  Chirp.Razor.Tests/         # unit and integrationn tests
  Chirp.PlaywrightTests/     # E2E browser tests
```

## License

MIT [LICENSE](LICENSE).
