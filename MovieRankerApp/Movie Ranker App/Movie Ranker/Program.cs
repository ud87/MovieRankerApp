using Microsoft.EntityFrameworkCore;
using Movie_Ranker.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddRazorOptions(options =>
    {
        // Add paths for custom view locations
        options.ViewLocationFormats.Clear(); // Clear default paths to avoid conflicts

        // Specify the location for Home views (Index.cshtml)
        options.ViewLocationFormats.Add(@"/Movie%20Ranker%20App/Movie%20Ranker/Views/Home/{0}.cshtml");

        // Specify the location for Movie views (Create, Delete, Edit, Index.cshtml)
        options.ViewLocationFormats.Add(@"/Movie%20Ranker%20App/Movie%20Ranker/Views/Movie/{0}.cshtml");

        // Specify the location for Shared views (_Layout.cshtml, etc.)
        options.ViewLocationFormats.Add(@"/Movie%20Ranker%20App/Movie%20Ranker/Views/Shared/{0}.cshtml");
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
