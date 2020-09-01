using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GZTimeServer.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using AspNetCoreRateLimit;

namespace GZTimeServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddOptions();
            //��appsettings.json��ȡ��Ӧ����
            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));

            //ע��������͹���洢
            services.AddSingleton<IIpPolicyStore, DistributedCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, DistributedCacheRateLimitCounterStore>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //���ã���������Կ��������
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            services.AddControllersWithViews();
            services.AddTransient<ILevelProcessRepository, LevelProcessRepository>();
            services.AddTransient<IAnimeProcessRepository, AnimeProcessRepository>();
            services.AddTransient<ICodeKeyRepository, CodeKeyRepository>();
            services.AddTransient<IMazeProcessRepository, MazeProcessRepository>();
            services.AddTransient<ILiveLikeRepository, LiveLikeRepository>();
            services.AddTransient<IRankRepository, RankRepository>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseIpRateLimiting();

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
