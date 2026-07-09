using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using QuickCommerce.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace QuickCommerce.Api.BackgroundWorkers
{
    public class SettlementWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SettlementWorker> _logger;

        public SettlementWorker(
            IServiceScopeFactory scopeFactory,
            ILogger<SettlementWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Settlement Worker Started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    var dashboardService =
                        scope.ServiceProvider.GetRequiredService<IAdminDashboardService>();

                    await dashboardService.GenerateDailySettlementAsync();

                    _logger.LogInformation("Daily settlements generated successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Settlement worker error");
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }
}