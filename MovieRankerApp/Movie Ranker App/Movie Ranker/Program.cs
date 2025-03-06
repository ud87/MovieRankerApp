using Microsoft.EntityFrameworkCore;
using Movie_Ranker.Data;

var options = new WebApplicationOptions
{
    ContentRootPath = "/app/MovieRankerApp/MovieRankerApp/Movie Ranker App/Movie Ranker"
};

var builder = WebApplication.CreateBuilder(options); // Pass the options here

// Debug log to check content root in Render logs
Console.WriteLine($"Content Root Path: {builder.Environment.ContentRootPath}");

// Configure services
builder.Services.AddControllersWithViews()
    .AddRazorOptions(options =>
    {
        options.ViewLocationFormats.Clear();
        options.ViewLocationFormats.Add("/Views/{1}/{0}.cshtml");
        options.ViewLocationFormats.Add("/Views/Shared/{0}.cshtml");

        // Absolute paths with /app/ prefix for Render deployment
        options.ViewLocationFormats.Add("/app/MovieRankerApp/Movie Ranker App/Movie Ranker/Views/Home/{0}.cshtml");
        options.ViewLocationFormats.Add("/app/MovieRankerApp/Movie Ranker App/Movie Ranker/Views/Movie/{0}.cshtml");
        options.ViewLocationFormats.Add("/app/MovieRankerApp/Movie Ranker App/Movie Ranker/Views/Shared/{0}.cshtml");
    });

var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("DATABASE_URL environment variable is not set.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
