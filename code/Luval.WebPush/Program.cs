var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

/*
 * REFERENCE ARTICLES: 
 *  ** https://github.com/web-push-libs/web-push-csharp
 *  ** https://blog.elmah.io/how-to-send-push-notifications-to-a-browser-in-asp-net-core/
 *  Create keys
 *  https://tools.reactpwa.com/vapid
 * 
 */

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
