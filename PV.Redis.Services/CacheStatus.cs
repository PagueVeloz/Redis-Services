using System;

namespace PV.Redis.Services
{
    internal sealed class CacheStatus
    {
        public bool CacheServerIsUp { get; set; }

        public DateTime LastReconnectionAttempt { get; set; }

        public static CacheStatus Information { get; } = new CacheStatus();

        private CacheStatus()
        {
        }
    }
}