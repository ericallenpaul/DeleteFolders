using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog;
using NLog.Web;

namespace DeleteFolders
{
    class Program
    {
        private const string _appsettings = "appsettings.json";
        private static FolderService _svc;
        private static readonly NLog.ILogger _logger = LogManager.GetCurrentClassLogger();
        private static readonly IConfiguration _configuration;

        static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    var baseDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                    Console.WriteLine($"Setting base directory to: {baseDirectory}");
                    //configApp.SetBasePath(Directory.GetCurrentDirectory());
                    configApp.SetBasePath(baseDirectory);
                    configApp.AddJsonFile(_appsettings, optional: true);
                    configApp.AddJsonFile(
                        $"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json",
                        optional: true);
                    configApp.AddCommandLine(args);
                    Console.WriteLine(hostContext.HostingEnvironment.EnvironmentName);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddScoped<FolderService>();
                    services.Configure<cliSettings>(Options =>
                        hostContext.Configuration.GetSection("cliSettings").Bind(Options));

                }).ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .UseNLog();

            var host = builder.Build();

            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;

                try
                {
                    _svc = services.GetRequiredService<FolderService>();

                    var helpWriter = new StringWriter();
                    var parser = new CommandLine.Parser(with => with.HelpWriter = helpWriter);
                    var cmdOptions = parser.ParseArguments<DeleteFolderOptions, PurgeOptions>(args);
                    cmdOptions.WithParsed<DeleteFolderOptions>(
                        options =>
                        {
                            int day = DateTime.Today.Day;
                            int month = DateTime.Today.Month;
                            if (options.renewalday > 0)
                            {
                                day = options.renewalday;
                            }

                            if (options.renewalmonth > 0)
                            {
                                month = options.renewalmonth;
                            }

                            if (day > 27)
                            {
                                _logger.Debug($"Day: {day} is greater than the max. Changing day to 27.");
                                day = 27;
                            }

                            _logger.Info($"Preparing to run renewals for Month: {month} Day: {day}");

                            Renew(day, month, options.includeerrors);

                        }).WithParsed<PurgeOptions>(purgeOptions => {

                            Purge();

                        }).WithNotParsed(errs =>
                        {
                            DisplayHelp(errs, helpWriter);
                        });

                    Environment.Exit(0);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Error: {ex}");
                }
            }
        }

        private static void DisplayHelp(IEnumerable<CommandLine.Error> errs, TextWriter helpWriter)
        {
            if (errs.IsVersion() || errs.IsHelp())
                Console.WriteLine(helpWriter.ToString());
            else
                Console.Error.WriteLine(helpWriter.ToString());
        }

        private static async void Renew(int renewalday, int renewalmonth, bool includeerrors)
        {
            //var renewals = await _client.Get_Expiring_Domains_by_Renewal_day_and_monthAsync(renewalday, renewalmonth);

            //foreach (var domain in renewals.Data.ToList())
            //{
            //    if (domain.ExceptionType == ExceptionTypes._8 || domain.ExceptionType == null)
            //    {
            //        _logger.Info($"Renewing domain: {domain.DomainName}");
            //        var result = await _client.Get_CertificateAsync(domain.DomainName, domain.Secure, "system");
            //    }
            //    else
            //    {
            //        _logger.Info($"Skipping failed domain: {domain.DomainName}");
            //    }
            //}


            await _svc.RenewCertificates(renewalday, renewalmonth, includeerrors);
        }

        private static async void Purge()
        {
            await _svc.PurgeCertificates();
        }
    }
}
