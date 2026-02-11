using System;

namespace StatisticPanel
{
    public class PlayerStats
    {
        public string Nickname { get; set; } = "Unknown";
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int TotalPlaySeconds { get; set; }
        public DateTime LastJoin { get; set; } = DateTime.Now;
        public DateTime LastSeenUtc { get; set; } = DateTime.UtcNow;
    }
}
