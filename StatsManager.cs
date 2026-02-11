using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Exiled.API.Features;

namespace StatisticPanel
{
    public class StatsManager
    {
        public Dictionary<string, PlayerStats> Stats = new();
        private readonly string dataPath;

        public StatsManager()
        {
            dataPath = Path.Combine(Paths.Plugins, "StatisticPanel", "stats.json");
            Directory.CreateDirectory(Path.GetDirectoryName(dataPath));

            if (File.Exists(dataPath))
            {
                Stats = JsonConvert.DeserializeObject<Dictionary<string, PlayerStats>>(File.ReadAllText(dataPath))
                        ?? new Dictionary<string, PlayerStats>();
            }
        }

        public void Save()
        {
            File.WriteAllText(dataPath, JsonConvert.SerializeObject(Stats, Formatting.Indented));
        }

        public string GetId(Player p) => p.UserId.Replace("@steam", "");
    }
}
