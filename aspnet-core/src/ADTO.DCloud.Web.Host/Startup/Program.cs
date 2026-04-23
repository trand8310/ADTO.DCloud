using ADTOSharp.AspNetCore.Dependency;
using ADTOSharp.Dependency;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;

namespace ADTO.DCloud.Web.Host.Startup
{
    public class Program
    {

        //public static void Main(string[] args)
        //{
        //    var builder = WebApplication.CreateBuilder(args);

        //    // -----------------------------
        //    // Kestrel 配置
        //    // -----------------------------
        //    builder.WebHost.ConfigureKestrel(opt =>
        //    {
        //        opt.AddServerHeader = false;
        //        opt.Limits.MaxRequestLineSize = 16 * 1024;
        //    });

        //    // -----------------------------
        //    // Content Root
        //    // -----------------------------
        //    builder.WebHost.UseContentRoot(Directory.GetCurrentDirectory());

        //    // -----------------------------
        //    // IIS 支持
        //    // -----------------------------
        //    builder.WebHost.UseIIS();
        //    builder.WebHost.UseIISIntegration();

        //    // -----------------------------
        //    // 日志过滤
        //    // -----------------------------
        //    builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);

        //    // -----------------------------
        //    // Startup 配置
        //    // -----------------------------
        //    var startup = new Startup(builder.Environment);

        //    // ConfigureServices
        //    startup.ConfigureServices(builder.Services);

        //    var app = builder.Build();

        //    // Configure
        //    startup.Configure(app, builder.Environment, app.Services.GetRequiredService<ILoggerFactory>());

        //    app.Run();
        //}


        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            // -----------------------------
            // Kestrel 配置
            // -----------------------------
            builder.WebHost.ConfigureKestrel(opt =>
            {
                opt.AddServerHeader = false;
                opt.Limits.MaxRequestLineSize = 16 * 1024;
            });

            // -----------------------------
            // Content Root
            // -----------------------------
            builder.WebHost.UseContentRoot(Directory.GetCurrentDirectory());

            // -----------------------------
            // IIS 支持
            // -----------------------------
            builder.WebHost.UseIIS();
            builder.WebHost.UseIISIntegration();


            // -----------------------------
            // 保留 CastleWindsor
            // -----------------------------
            builder.Host.UseCastleWindsor(IocManager.Instance.IocContainer);


            // -----------------------------
            // 日志过滤
            // -----------------------------
            builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);


            // -----------------------------
            // Startup 配置服务
            // -----------------------------
            var startup = new Startup(builder.Environment);
            startup.ConfigureServices(builder.Services);

            // -----------------------------
            // 构建 WebApplication
            // -----------------------------
            var app = builder.Build();

            // -----------------------------
            // Startup 配置管道
            // -----------------------------
            startup.Configure(app, builder.Environment, app.Services.GetRequiredService<ILoggerFactory>());

            app.Run();


            //CreateHostBuilder(args).Build().Run();
        }

        //internal static IHostBuilder CreateHostBuilder(string[] args) =>
        //    Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
        //.ConfigureWebHostDefaults(webBuilder =>
        //{
        //    webBuilder.UseStartup<Startup>();
        //})
        //.UseCastleWindsor(IocManager.Instance.IocContainer);


        //public static void Main(string[] args)
        //{
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
}
