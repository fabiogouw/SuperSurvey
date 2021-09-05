using Amazon.SQS;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SuperSurvey.Adapters;
using SuperSurvey.UseCases;
using SuperSurvey.UseCases.Ports.In;
using SuperSurvey.UseCases.Ports.Out;
using SuperSurvey.WebApp.HostedServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuperSurvey.WebApp
{
    public class Startup
    {
        private IWebHostEnvironment _currentEnv;
        public Startup(IConfiguration configuration, IWebHostEnvironment currentEnv)
        {
            Configuration = configuration;
            _currentEnv = currentEnv;
        }

        public IConfiguration Configuration { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (_currentEnv.IsDevelopment())
            {
                services.AddTransient<IAmazonSQS>(_ => new AmazonSQSClient("test", "test", new AmazonSQSConfig()
                {
                    ServiceURL = Configuration.GetConnectionString("AWSServiceURL")
                }));
            }
            else
            {
                services.AddTransient<IAmazonSQS, AmazonSQSClient>();
            }
            services.AddControllersWithViews();
            services.AddTransient<ManagePollsUseCase, ManagePollsUseCaseImpl>();
            services.AddTransient<CountVotesUseCase, CountVotesUseCaseImpl>();
            services.AddTransient<CastVoteUseCase, CastVoteUseCaseImpl>();
            services.AddTransient<ViewResultsUseCase, ViewResultsUseCaseImpl>();
            services.AddTransient<VoteCommandRepository>(svcProvider => 
                new SQSVoteRepository(svcProvider.GetRequiredService<IAmazonSQS>(), Configuration.GetConnectionString("VoteQueue")));
            services.AddTransient<PollRepository>(_ => new MySQLPollRepository(Configuration.GetConnectionString("PollDb")));
            services.AddHostedService<SQSVoteCounterHostedService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
