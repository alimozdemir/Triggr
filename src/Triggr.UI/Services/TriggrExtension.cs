using System.Runtime.InteropServices;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Internal;
using Microsoft.Extensions.DependencyInjection;
using Octokit;
using Triggr.Data;
using Triggr.Infrastructure;
using Triggr.Providers;
using Triggr.Services;

namespace Triggr.UI.Services
{
    public static class TriggrExtension
    {
        private const string TriggrJobId = "triggr";
        public static void AddTriggr(this IServiceCollection services)
        {
            services.AddSingleton<RepositoryStorage>(i => new RepositoryStorage("../repositories/"));
            services.AddSingleton<ScriptStorage>(i => new ScriptStorage("../Scripts/"));
            services.AddScoped<IContainerService, ContainerService>();

            services.AddScoped<IProvider, GitProvider>();

            services.AddScoped<IProviderFactory, ProviderFactory>();
            services.AddTransient<ProbeControl>();
            services.AddTransient<TController>();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                services.AddTransient<IShellExecutor, UnixExecutor>();
            }
            else
            {
                services.AddTransient<IShellExecutor, WindowsExecutor>();
            }

            services.AddTransient<IScriptExecutor, ScriptExecutor>();
            services.AddSingleton<ILanguageService, LanguageService>(i => new LanguageService("../config/languages.json"));

            services.AddScoped<IWebhookService, WebhookService>();

            services.AddScoped<IWebhookFactory, WebhookFactory>();

            //Octokit github client
            services.AddTransient<GitHubClient>(i => new GitHubClient(new ProductHeaderValue("Triggr")));

            services.AddSingleton<IBackgroundJobClient, BackgroundJobClient>();
            services.AddSingleton<Wrappers.GithubWrapper>();
            services.AddSingleton<LibGit2SharpWrapper>();

            services.AddScoped<IMessageFactory, MessageFactory>();
            services.AddScoped<IMessageService, EmailService>();
            services.AddScoped<IMessageService, IssueService>();
        }

        public static void UseTriggr(this IApplicationBuilder app, bool webHook = false)
        {
            /*if (!webHook)
                RecurringJob.AddOrUpdate<TController>(TriggrJobId, i => i.Tick(null), Cron.Minutely);*/
        }
    }

}