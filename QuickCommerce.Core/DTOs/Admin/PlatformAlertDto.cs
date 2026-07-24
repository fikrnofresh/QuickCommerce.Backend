using System;

namespace QuickCommerce.Core.DTOs.Admin
{
    public class PlatformAlertDto
    {
        public string AlertType { get; set; }

        public string Message { get; set; }

        public string Severity { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}