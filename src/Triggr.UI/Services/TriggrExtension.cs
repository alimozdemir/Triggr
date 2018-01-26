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
            services.AddSingleton<IStorage, Storage>(i => new Storage("../repositories/"));
            services.AddScoped<IContainerService, ContainerService>();
            
            services.AddScoped<IProvider, GitProvider>();
            services.AddScoped<IProvider, TestProvider>();

            services.AddScoped<IProviderFactory, ProviderFactory>();
            services.AddTransient<TController>();

        }

        public static void UseTriggr(this IApplicationBuilder app)
        {
            RecurringJob.AddOrUpdate<TController>(TriggrJobId, i => i.Tick(null), Cron.Minutely);
        }
    }

    public class TestProvider : IProvider
    {
        public string GetProviderType => throw new System.NotImplementedException();

        public bool IsValid(string url)
        {
            throw new System.NotImplementedException();
        }

        public string Update(Repository repository)
        {
            throw new System.NotImplementedException();
        }
    }
}