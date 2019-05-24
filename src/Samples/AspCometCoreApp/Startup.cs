using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using static AspComet.Middleware.CometExtensions;
using AspComet;
using AspComet.Eventing;

namespace AspCometCoreApp
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }


        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();


        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureBasicCometServices();

            services.AddSingleton<IClientFactory, AuthenticatedClientFactory>();
            services.AddSingleton<HandshakeAuthenticator>();
            services.AddSingleton<BadLanguageBlocker>();
            services.AddSingleton<SubscriptionChecker>();
            services.AddSingleton<Whisperer>();
   

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // Create branch to the CometMiddleware. 
            // All requests ending in comet/45.0 will follow this branch.
            app.MapWhen(
                context => context.Request.Path.ToString().Contains("/comet/45.0"),
                appBranch => {
                    // ... optionally add more middleware to this branch
                    appBranch.UseCometMiddleware();//formerly an 'HttpHandler'
                });

            

            EventHub.Subscribe<HandshakingEvent>(app.ApplicationServices.GetService<HandshakeAuthenticator>().CheckHandshake);
            EventHub.Subscribe<PublishingEvent>(app.ApplicationServices.GetService<BadLanguageBlocker>().CheckMessage);
            EventHub.Subscribe<SubscribingEvent>(app.ApplicationServices.GetService<SubscriptionChecker>().CheckSubscription);
            EventHub.Subscribe<PublishingEvent>("/service/whisper", app.ApplicationServices.GetService<Whisperer>().SendWhisper);

            app.UseFileServer();
        }
    }
}
