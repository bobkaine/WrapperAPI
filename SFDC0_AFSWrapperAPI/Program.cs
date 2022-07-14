using WrapperAPI.Models.Configuration;
using NLog;
using NLog.Extensions.Hosting;
using NLog.Extensions.Logging;

namespace WrapperAPI
{
    public static class Program
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private static readonly string _environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
        private static readonly IConfigurationRoot _config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                                    .AddJsonFile($"appsettings.{_environment}.json", optional: true, reloadOnChange: true)
                                                    .Build();
        public static async Task Main()
        {
            try
            {
                var host = CreateHostBuilder().Build();

                await AddNLogConfigurationAsync(host);

                _logger.Debug("Starting AFS Wrapper API");

                host.Run();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                _logger.Fatal(ex, "AFS Wrapper API terminated unexpectedly because of exception");
            }
            finally
            {
                LogManager.Shutdown();
            }


        }

        private static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();

                    if (_environment == Environments.Development || (!RunningInIIS() && _environment != Environments.Development))
                    {
                        logging.AddConsole();
                    }

                    if (_environment == Environments.Production)
                    {
                        logging.AddEventLog();
                    }

                    logging.AddConfiguration(_config.GetSection("Logging"));
                })
                .UseNLog();
        }

        private static bool RunningInIIS() =>
            Environment.GetEnvironmentVariable("APP_POOL_ID") != null;

        private static async Task AddNLogConfigurationAsync(IHost host)
        {
            LogManager.Configuration = new NLogLoggingConfiguration(_config.GetSection("NLog"));

            using (var scope = host.Services.CreateScope())
            {
                await NLogConfiguration.UpdateNetworkAddressAsync(scope.ServiceProvider, "loggingMicroservice");
            }

            LogManager.ReconfigExistingLoggers();
        }
    }
        
 
}
