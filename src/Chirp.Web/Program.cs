using Chirp.Core.Repositories;
using Chirp.Infrastructure;
using Chirp.Infrastructure.Chirp.Repositories;
using Chirp.Infrastructure.Chirp.Services;
using Chirp.Infrastructure.Identity;
using Chirp.Web.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var seedOnly = args.Contains("--seed-only", StringComparer.OrdinalIgnoreCase);
    
// loading the db connection via the configuration
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ChirpDbContext>(options => options.UseSqlite(connectionString, b => b.MigrationsAssembly("Chirp.Infrastructure")));

// adding services to the container
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ChirpDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();
builder.Services.AddTransient<IEmailSender, NullEmailSender>();

// configuring github OAuth if the credentials are there
var githubClientId = builder.Configuration["Authentication:GitHub:ClientId"];
var githubClientSecret = builder.Configuration["Authentication:GitHub:ClientSecret"];

var authBuilder = builder.Services.AddAuthentication();

if (!string.IsNullOrEmpty(githubClientId) && !string.IsNullOrEmpty(githubClientSecret))
{
    authBuilder.AddGitHub(options =>
    {
        options.ClientId = githubClientId;
        options.ClientSecret = githubClientSecret;
        options.CallbackPath = "/signin-github";
    });
}

builder.Services.AddRazorPages();
builder.Services.AddScoped<CheepService>();
builder.Services.AddScoped<ICheepRepository, CheepRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();


var app = builder.Build();

// configuring http request pipelinne
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // default hsts value is 30 days, so possibly we should change this for prod scenarios (https://aka.ms/aspnetcore-hsts).
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ChirpDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // ensuring that the db is created + migrated
    db.Database.Migrate();

    // seeding db
    DbInitializer.SeedDatabase(db, userManager);
}

if (seedOnly)
{
    return;
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.Use(async (context, next) =>
{
    if (context.User?.Identity?.IsAuthenticated == true)
    {
        var authorRepository = context.RequestServices.GetRequiredService<IAuthorRepository>();
        var userManager = context.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
        var user = await userManager.GetUserAsync(context.User);
        if (user != null)
        {
            var authorName = user.UserName ?? user.Email ?? "unknown-user";
            var authorEmail = user.Email ?? $"{authorName}@chirp.dk";
            await authorRepository.MakeSureAuthorExists(authorName, authorEmail);
        }
    }

    await next();
});
app.UseAuthorization();

app.MapRazorPages();

app.Run();
