using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Exiled.API.Features;

namespace StatisticPanel
{
    public static class SupabaseSender
    {
        private static readonly HttpClient http = new();

        public static async void Send(string steamId, PlayerStats ps, StatsConfig config)
        {
            if (string.IsNullOrWhiteSpace(config.SupabaseUrl) || string.IsNullOrWhiteSpace(config.SupabaseKey))
            {
                Log.Warn("Supabase configuration missing");
                return;
            }

            try
            {
                var json = JsonConvert.SerializeObject(new
                {
                    steam_id = steamId,
                    nickname = ps.Nickname,
                    total_play_seconds = ps.TotalPlaySeconds,
                    kills = ps.Kills,
                    deaths = ps.Deaths,
                    last_seen_utc = ps.LastSeenUtc.ToString("o")
                });

                var req = new HttpRequestMessage(HttpMethod.Post, $"{config.SupabaseUrl}/rest/v1/player_stats?on_conflict=steam_id")
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };

                req.Headers.Add("apikey", config.SupabaseKey);
                req.Headers.Add("Authorization", $"Bearer {config.SupabaseKey}");
                req.Headers.Add("Prefer", "resolution=merge-duplicates");

                var res = await http.SendAsync(req);

                if (!res.IsSuccessStatusCode)
                    Log.Warn($"Supabase error: {res.StatusCode} - {await res.Content.ReadAsStringAsync()}");
            }
            catch (Exception ex)
            {
                Log.Error($"Supabase HTTP error: {ex.Message}");
            }
        }
    }
}
