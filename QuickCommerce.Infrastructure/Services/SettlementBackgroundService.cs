using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using QuickCommerce.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace QuickCommerce.Infrastructure.Services
{
    public class SettlementBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SettlementBackgroundService> _logger;

        public SettlementBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<SettlementBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Settlement Background Service Started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var now = DateTime.UtcNow;

                    var nextRun = DateTime.UtcNow.Date.AddDays(1).AddHours(2);

                    var delay = nextRun - now;

                    await Task.Delay(delay, stoppingToken);

                    using var scope = _serviceProvider.CreateScope();

                    var dashboardService = scope.ServiceProvider
                        .GetRequiredService<IAdminDashboardService>();

                    await dashboardService.GenerateDailySettlementAsync();

                    _logger.LogInformation("Daily Settlement Generated");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Settlement Job Failed");
                }
            }
        }
    }
}