using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataService.Utils;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace PBSA_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
            //use this to allow command line parameters in the config
            //var configuration = new ConfigurationBuilder()
            //    .AddCommandLine(args)
            //    .Build();

            //var hostUrl = configuration["hosturl"];
            //if (string.IsNullOrEmpty(hostUrl))
            //    hostUrl = $"http://{CommonUtil.GetLocalIPAddress()}:5000";

            //var host = new WebHostBuilder()
            //    .UseKestrel()                
            //    .UseUrls(hostUrl)   // <!-- this 
            //    .UseContentRoot(Directory.GetCurrentDirectory())
            //    .UseIISIntegration()
            //    .UseStartup<Startup>()
            //    .UseConfiguration(configuration)
            //    .Build();

            //host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
