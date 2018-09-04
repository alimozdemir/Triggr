using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Console;
using Hangfire.SQLite;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Triggr.UI.Services;
using Triggr.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.HttpOverrides;

namespace Triggr.UI
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("emailsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TriggrContext>(i =>
            {
                i.UseSqlite(Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("Triggr.UI"));
            });


            services.AddMvc();
            //.SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
            //.AddGitHubWebHooks();

            services.AddHangfire(i =>
            {
                var options = new SQLiteStorageOptions();
                i.UseSQLiteStorage(Configuration.GetConnectionString("HangfireConnection"), options);
                i.UseActivator(new HangfireActivator(services));
                i.UseConsole();
            });

            var config = Configuration.GetSection("TriggrConfig");
            var email = Configuration.GetSection("EmailConfig");

            services.Configure<Triggr.TriggrConfig>(config);

            services.Configure<Triggr.EmailConfig>(email);

            services.AddTriggr();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseDeveloperExceptionPage();
                var serviceScopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
                using (var serviceScope = serviceScopeFactory.CreateScope())
                {
                    var dbContext = serviceScope.ServiceProvider.GetService<TriggrContext>();
                    dbContext.Database.EnsureCreated();
                } 
                //app.UseExceptionHandler("/Home/Error");
                //app.UseHsts();
            }

            // for development on Linux Ubuntu 16.04
            /*app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });*/


            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // set the workercount because of sqlite
            BackgroundJobServerOptions serverOptions = new BackgroundJobServerOptions();
            serverOptions.WorkerCount = 1;
            app.UseHangfireServer(serverOptions);
            app.UseHangfireDashboard();
            app.UseTriggr(true);
        }
    }
}
