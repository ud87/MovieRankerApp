using Microsoft.EntityFrameworkCore;
using Movie_Ranker.Data;

var builder = WebApplication.CreateBuilder(); // Pass the options here
Console.WriteLine($"Content Root Path: {builder.Environment.ContentRootPath}");

// Debug log to check content root in Render logs
Console.WriteLine($"Content Root Path: {builder.Environment.ContentRootPath}");
Console.WriteLine($"Current Directory: {Directory.GetCurrentDirectory()}");

// Configure services
builder.Services.AddControllersWithViews()
    .AddRazorOptions(options =>
    {
        options.ViewLocationFormats.Clear();
        options.ViewLocationFormats.Add("/Views/{1}/{0}.cshtml");
        options.ViewLocationFormats.Add("/Views/Shared/{0}.cshtml");

    });

var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");

if (string.IsNullOrEmpty(connectionString))
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection"); //for local development
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
