using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Internal;
using Microsoft.Extensions.DependencyInjection;
using Triggr.Data;
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
            services.AddTransient<IScriptExecutor, ScriptExecutor>();
            services.AddSingleton<ILanguageService, LanguageService>(i => new LanguageService("../config/languages.json"));

        }

        public static void UseTriggr(this IApplicationBuilder app)
        {
            RecurringJob.AddOrUpdate<TController>(TriggrJobId, i => i.Tick(null), Cron.Minutely);
        }
    }

}