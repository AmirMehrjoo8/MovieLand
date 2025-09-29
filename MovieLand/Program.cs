using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MovieLand.Data.Repository;
using MovieLand.Data.Service;
using MovieLand.Models.Context;
using System.Globalization;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
#region multi lang

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "en-US", "fa-IR" }
        .Select(c => new CultureInfo(c))
        .ToList();

    options.DefaultRequestCulture = new RequestCulture("fa-IR");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

#endregion

#region db
builder.Services.AddDbContext<MovieLandDbContext>(options =>
options.UseSqlServer("Data Source=.;Initial Catalog=MovieLand_DB;Integrated Security=True;TrustServerCertificate=True;"));
#endregion

#region Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICommentRepository, CommentReposetory>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IDiscountCodeRepository, DiscountCodeRepository>();
#endregion

#region Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.LoginPath = "/Account/Login";
        option.LogoutPath = "/Account/LogOut";
        option.ExpireTimeSpan = TimeSpan.FromDays(30);
    });
#endregion
builder.Services.AddHttpClient();
builder.WebHost.ConfigureKestrel(options =>
{
    options.AllowSynchronousIO = true;
});
var app = builder.Build();
#region multi lang
var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
app.UseRequestLocalization(localizationOptions);
#endregion
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/Admin"))
    {
        if (!context.User.Identity.IsAuthenticated)
        {
            context.Response.Redirect("/Account/Login");
        }
        else if (context.User.FindFirstValue("Type") != "1")
        {
            context.Response.Redirect("/Account/Login");
        }
    }
    await next.Invoke();
});

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}/{code?}");

app.Run();
