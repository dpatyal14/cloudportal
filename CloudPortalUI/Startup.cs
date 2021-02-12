using System;
using System.Collections.Generic;

using System.Linq;
using System.Threading.Tasks;
using CloudPortal.Models;
//using CloudPortal.Repository;
using CloudPortal.Services;
using CloudPortalRepository.Repository;
using CloudPortalServices.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MySqlConnector;

namespace CloudPortal
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddTransient<MySqlConnection>(_ => new MySqlConnection(Configuration["ConnectionStrings:Default"]));
        //   services.Add(new ServiceDescriptor(typeof(AccountRepository), new AccountRepository(Configuration["ConnectionStrings:Default"])));
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IDocumentListService, DocumentListService>();
            services.AddScoped<IDocumentListRepository, DocumentListRepository>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
            });

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
            }
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();
            app.UseAuthorization();
          //  app.UseMiddleware<JwtMiddleware>();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Account}/{action=Login}/{id?}");
            });
        }
    }
}
