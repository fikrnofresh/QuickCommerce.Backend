using System;

namespace QuickCommerce.Core.DTOs.Admin
{
    public class SessionDto
    {
        public string SessionId { get; set; } = string.Empty;

        public string DeviceName { get; set; } = string.Empty;

        public string Browser { get; set; } = string.Empty;

        public string OperatingSystem { get; set; } = string.Empty;

        public string IpAddress { get; set; } = string.Empty;

        public bool IsCurrentSession { get; set; }

        public DateTime LoginTime { get; set; }

        public DateTime? LastActivity { get; set; }
    }
}