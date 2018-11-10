using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace RealtimeChatSample.Api
{
    public class Program
    {
        //public static void Main(string[] args)
        //{

        //    //kestrel
        //    /*var config = new ConfigurationBuilder()
        //        .AddJsonFile("hosting.json", optional: true)
        //        .SetBasePath(Directory.GetCurrentDirectory())
        //        .AddCommandLine(args)
        //        .Build();*/

        //    //var host = WebHost.CreateDefaultBuilder(args)
        //    //          .UseUrls("http:localhost:56837")
        //    //  .UseContentRoot(Directory.GetCurrentDirectory())
        //    //  .UseStartup<Startup>()
        //    //  .UseKestrel()
        //    //  .Build();

        //    //host.Run();
        //}


        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                  .UseKestrel()
                   .UseUrls("http://0.0.0.0:5000")
                  .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()

                .Build();


    }



}

