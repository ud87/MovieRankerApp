using Microsoft.EntityFrameworkCore;
using Movie_Ranker.Data;

var builder = WebApplication.CreateBuilder(args);

// Set the content root directory to match your Render.com folder structure
builder.Host.UseContentRoot("/app/MovieRankerApp/Movie Ranker App/Movie Ranker");

// Add MVC services and configure Razor view locations
builder.Services.AddControllersWithViews()
    .AddRazorOptions(options =>
    {
        // DO NOT REMOVE DEFAULT PATHS
        options.ViewLocationFormats.Add("/Views/{1}/{0}.cshtml");
        options.ViewLocationFormats.Add("/Views/Shared/{0}.cshtml");

        // Add your custom view locations
        options.ViewLocationFormats.Add("/app/MovieRankerApp/Movie Ranker App/Movie Ranker/Views/Home/{0}.cshtml");
        options.ViewLocationFormats.Add("/app/MovieRankerApp/Movie Ranker App/Movie Ranker/Views/Movie/{0}.cshtml");
        options.ViewLocationFormats.Add("/app/MovieRankerApp/Movie Ranker App/Movie Ranker/Views/Shared/{0}.cshtml");
    });


var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL"); //environment set in Render with formatted string

if ( string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("DATABASE_URL environment variable is not set.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString)
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");  //changed from Home/Error to /Error to fix the error on deployment
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); //added to serve static files like images, css, and js
app.UseRouting();

app.UseAuthorization();

// Remove the line causing the error
// app.MapStaticAssets();



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
