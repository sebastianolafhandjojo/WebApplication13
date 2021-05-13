using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

                    if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DYNO")))
                    {
                        int option = 1;
                        if (option == 1)
                        {
                            string dynoport = Environment.GetEnvironmentVariable("PORT");
                            string useUrl = $"http://*:{dynoport}";
                            webBuilder.UseUrls(useUrl);
                        }
                        else
                        {
                            if (!int.TryParse(Environment.GetEnvironmentVariable("PORT"), out var port))
                            { port = 5000; }
                            webBuilder.UseKestrel(options =>
                            {
                                options.Listen(IPAddress.Any, port);
                            });
                        }

                    }

                });
    }
}
