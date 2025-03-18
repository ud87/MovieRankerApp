using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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

//Add identity service to the application
builder.Services.AddIdentity<IdentityUser, IdentityRole>() //IdentityUser and IdentityRole is the default user class in ASP.NET Core Identity
    .AddEntityFrameworkStores<ApplicationDbContext>()      //configures Identity to use Entity Framework Core for storing user data
    .AddDefaultTokenProviders(); //adds token providers to generate tokens for password reset, email confirmation etc

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
app.UseAuthentication(); //Add authentication service to the application
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//Use the PORT environment variable provided by Render
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://0.0.0.0:{port}");

app.Run();
