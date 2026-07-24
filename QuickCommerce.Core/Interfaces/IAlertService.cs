using QuickCommerce.Core.DTOs.Admin;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuickCommerce.Core.Interfaces
{
    public interface IAlertService
    {
        Task<List<PlatformAlertDto>> GetPlatformAlertsAsync();
    }
}