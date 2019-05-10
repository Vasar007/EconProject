using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace EconProject.CurrencyWebService
{
    public static class Program
    {
        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                       .ConfigureAppConfiguration((hostingContext, config) =>
                       {
                           config.AddJsonFile("appsettings.json",
                                              optional: false,
                                              reloadOnChange: false);
                       })
                       .UseStartup<Startup>();
        }

        private static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }
    }
}
