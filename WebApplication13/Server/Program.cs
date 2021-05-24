using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Serilog;

namespace WebApplication13.Server
{
    public class Program
    {  
        public static void Main(string[] args)
        {
            SetupSerilog();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureServices((ctx, svc) =>
                {
                    HostConfig.CertPath = ctx.Configuration["CertificateFileLocation"];
                    HostConfig.CertificatePassword = ctx.Configuration["CertPassword"];
                    Log.Information($"HostConfig.CertPath           : {HostConfig.CertPath }");
                    Log.Information($"HostConfig.CertificatePassword: {HostConfig.CertificatePassword  }");
                    
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // 
                    IPHostEntry  host = System.Net.Dns.GetHostEntry("mysite2.com");

                    int i = 0;
                    Log.Information($"AddressList.IPAddress {host.AddressList.Length}");
                    foreach(IPAddress ip in host.AddressList)
                    {
                        Log.Information($"{i}) {ip.ToString()}");
                        i++;
                    }

                    webBuilder.ConfigureKestrel(opt =>
                    {
                        // somehow this laptop has invalid IP address : 172.96.186.187
                        // thatp keep detected as primary IP, 
                        // so temporary solution is to hardcode it to 192.168.1.82 for now,
                        // please check ipaddress.exe to see assigned IPAddress from real Router / WIFI

                        opt.Listen(IPAddress.Parse("192.168.1.82"), 5000); 
                        // above meaning accessable on browser http://192.168.1.82:5000 or http://handjojo.io:5000
                        
                        //opt.ListenAnyIP(5000);
                        opt.ListenAnyIP(5001, listOpt =>
                        {
                            listOpt.UseHttps(HostConfig.CertPath, HostConfig.CertificatePassword);
                        }); // meaning http://handjojo.io:5001

                        // if there is an "app.UseHttpsRedirection(); in startup class"
                        // meaning it will redirect http://handjojo.io:5000 to https://handjojo.io:5001

                    });
                    webBuilder.UseStartup<Startup>();


                    //if (Settings.IsDeployToHeroku)
                    //{
                    //    int option = 0;

                    //    if (option == 1)
                    //    {
                    //        string dynoport = Environment.GetEnvironmentVariable("PORT");
                    //        string useUrl = $"http://*:{dynoport}";
                    //        webBuilder.UseUrls(useUrl);
                    //    }
                    //    else if (option == 2)
                    //    {
                    //        if (!int.TryParse(Environment.GetEnvironmentVariable("PORT"), out var port))
                    //        { port = 5000; }
                    //        webBuilder.UseKestrel(options =>
                    //        {
                    //            options.Listen(IPAddress.Any, port);
                    //        });
                    //    }

                    //}

                });

        private static void SetupSerilog()
        {
            string logdir = "log";
            if (!Directory.Exists(logdir))
                Directory.CreateDirectory(logdir);

            var logfile = Path.Combine(logdir, $"{ new FileInfo(System.Reflection.Assembly.GetCallingAssembly().Location).Name }.log") ;
            
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(logfile, rollingInterval: RollingInterval.Day)
                .CreateLogger();

            /*
    *  appsettings.json 
    *  microsft own logging, put this back if
    * "Logging": {
       "LogLevel": {
     "Default": "Information",
     "Microsoft": "Warning",
     "Microsoft.Hosting.Lifetime": "Information"
   }
 },
   */
        }
    }


    public static class HostConfig
    {
        public static string CertPath { get; set; }
        public static string CertificatePassword { get; set; }
    }
    
    
   
}
