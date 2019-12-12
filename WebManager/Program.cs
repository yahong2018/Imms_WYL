using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Imms.WebManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();

        // public static void Main(string[] args)
        // {
        //     var pfxFile = Path.Combine(Directory.GetCurrentDirectory(), "imms-net.pfx");

        //     IWebHostBuilder host = WebHost.CreateDefaultBuilder(args)
        //         .UseStartup<Startup>()
        //         .ConfigureKestrel((context, options) =>
        //         {
        //             options.Listen(IPAddress.Loopback, 5000);
        //             options.Listen(IPAddress.Loopback, 5001, listenOptions =>
        //             {
        //                 listenOptions.UseHttps(pfxFile, "password@12345qwert");
        //             });
        //         });

        //     host.Build().Run();
        // }       
    }
}
