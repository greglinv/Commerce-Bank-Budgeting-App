using BlazorApp.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BlazorApp.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContextFactory<BlazorAppContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("BlazorAppContext") ?? throw new InvalidOperationException("Connection string 'BlazorAppContext' not found."),
        // Azure SQL (especially the free/serverless tier) auto-pauses when idle and the first
        // connection after a pause can fail while it resumes. Retry transient failures instead of erroring.
        sql => sql.EnableRetryOnFailure(maxRetryCount: 6, maxRetryDelay: TimeSpan.FromSeconds(10), errorNumbersToAdd: null)));

builder.Services.AddQuickGridEntityFrameworkAdapter();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();


builder.Services.AddSingleton<BlazorApp.Services.OllamaService>();




// Authentication?
// I was following this video here: https://www.youtube.com/watch?v=GKvEuA80FAE
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "auth_token"; // Custom cookie name
        options.LoginPath = "/login";
        options.Cookie.MaxAge = TimeSpan.FromDays(7);
        options.AccessDeniedPath = "/access-denied";
    });
// Authorization
builder.Services.AddAuthorization();
// Passing the authentication state thoughout the application
builder.Services.AddCascadingAuthenticationState();


//Blazor Bootstrap service
builder.Services.AddBlazorBootstrap();


// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    // Development-only: lets the EF error page apply pending migrations. Must not be exposed in production.
    app.UseMigrationsEndPoint();
}

// The scaffolded CRUD pages (/useraccounts, /transactions, /bankaccounts, /budgetplans) are debug tools
// that show and edit every user's data, including password hashes. Only serve them in Development,
// unless explicitly re-enabled with the "EnableDebugPages" setting.
if (!app.Environment.IsDevelopment() && !app.Configuration.GetValue<bool>("EnableDebugPages"))
{
    string[] debugPrefixes = ["/useraccounts", "/transactions", "/bankaccounts", "/budgetplans"];
    app.Use(async (context, next) =>
    {
        if (debugPrefixes.Any(p => context.Request.Path.StartsWithSegments(p, StringComparison.OrdinalIgnoreCase)))
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            return;
        }
        await next();
    });
}

app.UseHttpsRedirection();

app.UseStaticFiles();

// Also for authentication and authorization
// https://www.youtube.com/watch?v=GKvEuA80FAE
app.UseAuthentication();
app.UseAuthorization();
// For some reason this has to be after authentication. If antiforgery issues are giving you trouble, this is the place to temporarily (or permanently) comment out
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
