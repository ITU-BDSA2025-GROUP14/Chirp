var builder = WebApplication.CreateBuilder(args);

// Configure for Azure App Service
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
builder.WebHost.UseUrls($"http://*:{port}");

// adding services to the contiainer
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Debug logging for Azure
Console.WriteLine($"Starting application on port: {port}");
Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");
Console.WriteLine($"CSV file exists: {File.Exists("chirp_cli_db.csv")}");

// configuring the http request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

// Add a basic health check endpoint
app.MapGet("/", () => "Chirp WebService is running!");

Console.WriteLine("Application configured, starting server...");
app.Run();

public partial class Program { }