using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using WebApplication13.Server.Data;
using WebApplication13.Server.Models;

namespace WebApplication13.Server
{
    public class Startup
    {
       
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
      
            if (Settings.IsDeployToIIS)
            {
                services.Configure<IISServerOptions>(options =>
                {
                    options.AutomaticAuthentication = false;
                    //options.AuthenticationDisplayName = null;
                    //options.AllowSynchronousIO = false;
                    //options.MaxRequestBodySize = 30000000;
                });
            }
            if (Settings.IdentityServerDB.Equals(IdentityServerDBEnum.SqlServer))
            {
                services.AddDbContext<ApplicationDbContext> (options =>
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
                );                
            }
            else if (Settings.IdentityServerDB.Equals(IdentityServerDBEnum.Sqlite))
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlite (Configuration.GetConnectionString("DefaultConnection2"))
                );
            }
            else
            {
                Console.WriteLine("Doesn't user IdentityServer DB");
            }
            
            //services.AddHttpsRedirection(options => { options.HttpsPort = 5001; });
            //services.AddMvcCore();

            if (Settings.UseCors)
            {
                services
                    .AddCors(options =>
                    {
                        options.AddPolicy(Settings.CorsPolicy,
                            builder => builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader());
                    });
            }
            //services.Configure<ForwardedHeadersOptions>(options =>
            //{
            //    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
            //                               ForwardedHeaders.XForwardedProto;
            //    options.KnownNetworks.Clear();
            //    options.KnownProxies.Clear();
            //});

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddIdentityServer()            
                .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

            services.AddAuthentication()
                .AddIdentityServerJwt();            

            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
          
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DYNO")))
            //{
            //Console.WriteLine("Use https redirection");
            //app.UseForwardedHeaders();
            //app.UseHttpsRedirection();
            //}

            app.UseHttpsRedirection();            
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();
            if (Settings.UseCors)
            {
                app.UseCors(Settings.CorsPolicy);
            }

            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
