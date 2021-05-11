using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication13.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();


#if RELEASE
                    string dockerPort = Environment.GetEnvironmentVariable("PORT") ?? "8080";
                    //Console.Write($"Docker Port: {dockerPort}");
                    
                    string useUrl = $"http://*:{dockerPort}";                    
                    webBuilder.UseUrls(useUrl);
#endif

                });
    }
}
