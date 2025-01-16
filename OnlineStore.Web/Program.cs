using Microsoft.EntityFrameworkCore;
using OnlineStore.Web.Data;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using OnlineStore.Web.Binders;
using Microsoft.AspNetCore.Hosting;


var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllersWithViews(options =>
{
    options.ModelBinderProviders.Insert(0, new CustomDecimalModelBinderProvider());
});

if (builder.Environment.IsEnvironment("Test"))
{
    builder.Services.AddDbContext<OnlineStoreContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("OnlineStoreTestDB")));
}
else
{
    builder.Services.AddDbContext<OnlineStoreContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("OnlineStoreDB")));
}

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

var defaultCulture = new CultureInfo("pl-PL");
var supportedCultures = new[] { defaultCulture };

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(defaultCulture),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Unhandled exception: {ex.Message}");
        throw;
    }
});

app.Run();

public partial class Program { }

