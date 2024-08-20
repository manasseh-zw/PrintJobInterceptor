using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


CreateHostBuilder(args).Build().Run();

IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
          .UseWindowsService(options =>
          {
              options.ServiceName = "Sybrin.Net Printer Service";
          })
          .ConfigureServices((hostContext, services) =>
          {
              services.AddHostedService<PrintServerService>();
          });