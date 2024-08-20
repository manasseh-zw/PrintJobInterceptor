using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

public class PrintServerService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            Logger.Log("Starting Sybrin.Net Printer Service");
            var server = new PrintServer();
            await server.Start(stoppingToken);
        }
        catch (Exception ex)
        {
            Logger.Log($"Error in PrintServerService: {ex.Message}", Logger.LogLevel.Error);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        Logger.Log("Sybrin.Net Printer Service");
        await base.StopAsync(cancellationToken);
    }
}