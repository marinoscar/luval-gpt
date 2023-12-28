using Luval.WebGPT.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Authentication;
using Luval.WebGPT;
using Org.BouncyCastle.Security;
using Luval.GPT.Data;
using Luval.GPT.Data.Entities;
using Luval.GPT.Services;
using Luval.Framework.Core;
using Luval.Framework.Core.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add controllers
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

// Add services
builder.Services
    .AddConfing()
    .AddLogger()
    .AddDbContext()
    .AddGoogleAuth()
    .AddRepositories()
    .AddServices()
    .AddAppHostedServices()
    .AddCacheProviders()
    .AddPresenters();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// Required for issue:https://github.com/googleapis/google-api-dotnet-client/issues/1899#issuecomment-885669909
app.UseForwardedHeaders();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseAuthentication();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

//app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


var e = DateTime.UtcNow.ToString().Encrypt(ConfigManager.Get("EncryptionKey"));
Console.WriteLine(e);
var d = e.Decrypt(ConfigManager.Get("EncryptionKey"));
Console.WriteLine(d);



app.Run();