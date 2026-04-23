using ADTOSharp.AspNetCore.Dependency;
using ADTOSharp.Dependency;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ADTO.AuthServer.Startup;

public class Program
{

    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    internal static IHostBuilder CreateHostBuilder(string[] args) =>
        Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseStartup<Startup>();
    })
    .UseCastleWindsor(IocManager.Instance.IocContainer);



    //public static void Main(string[] args)
    //{
    //    //CreateHostBuilder(args).Build().Run();
    //    CreateWebHostBuilder(args).Build().Run();
    //}

    //public static IWebHostBuilder CreateWebHostBuilder(string[] args)
    //{
    //    return new WebHostBuilder()
    //        .UseKestrel(opt =>
    //        {
    //            opt.AddServerHeader = false;
    //            opt.Limits.MaxRequestLineSize = 16 * 1024;
    //        })
    //        .UseContentRoot(Directory.GetCurrentDirectory())
    //        .ConfigureLogging((context, logging) =>
    //        {
    //            logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
    //        })
    //        .UseIIS()
    //        .UseIISIntegration()
    //        .UseStartup<Startup>();
    //}


}
