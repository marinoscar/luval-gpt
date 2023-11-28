using Luval.Framework.Core.Configuration;
using Luval.GPT.WebApi.Config;
using Luval.GPT.Logging;
using IConfigurationProvider = Luval.Framework.Core.Configuration.IConfigurationProvider;
using NLog;
using Amazon.Runtime;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using Luval.GPT.Channels;
using Luval.GPT.Services;
using Luval.OpenAI.Chat;
using Luval.GPT.Data;
using Luval.GPT.GPT;
using Luval.GPT.GPT.OpenAI;

namespace Luval.GPT.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            //Add application services
            var config = AppUtils.GetConfigurationProvider();
            var logger = AppUtils.GetLogger(config);

            builder.Services.AddSingleton<IConfigurationProvider>(config);
            builder.Services.AddSingleton<ILogger>(logger);
            builder.Services.AddSingleton<AppRepository>(AppUtils.GetAndInitAppRepo(config));
            builder.Services.AddSingleton<IMessageClient>((s) => AppUtils.GetMessageClient(config));
            builder.Services.AddScoped<ChatEndpoint>((s) => AppUtils.GetChatEndpoint(config));
            builder.Services.AddScoped<IChatAgent, OpenAIChatAgent>();
            builder.Services.AddScoped<ChatAgentService>();

            builder.Services.AddHostedService<Scheduler>();

            logger.LogInformation("Starting Service");

            var app = builder.Build();

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

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
