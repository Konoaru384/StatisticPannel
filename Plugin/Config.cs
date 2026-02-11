using Exiled.API.Interfaces;
using System.ComponentModel;

namespace StatisticPanel
{
    public class StatsConfig : IConfig
    {
        [Description("----------------------- CONFIGURATION -----------------------")]
        public bool IsEnabled { get; set; } = true;
        [Description("DEBUG VERSION COMING SOON...")]
        public bool Debug { get; set; } = false;
        [Description("Ur Supabase URL (read plugin description on github)")]
        public string SupabaseUrl { get; set; } = "";

        [Description("Ur Supabase ANON KEY (read plugin description on github)")]

        public string SupabaseKey { get; set; } = "";

        [Description("Did the plugin Respect DNT ? (change to false will break Nothwood Server Guildness)")]

        public bool RespectDNT { get; set; } = true;
    }
}
