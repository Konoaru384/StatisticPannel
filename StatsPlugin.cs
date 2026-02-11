using System;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;

namespace StatisticPanel
{
    public class StatsPlugin : Plugin<StatsConfig>
    {
        public override string Name => "StatisticPanel";
        public override string Author => "Konoara";
        public override Version Version => new(2, 0, 0);
        public override Version RequiredExiledVersion => new(9, 10, 0);

        private StatsManager manager;

        public override void OnEnabled()
        {
            manager = new StatsManager();

            Exiled.Events.Handlers.Player.Verified += OnJoin;
            Exiled.Events.Handlers.Player.Left += OnLeft;
            Exiled.Events.Handlers.Player.Died += OnDied;
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

            Log.Info("StatisticPanel enabled");
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Verified -= OnJoin;
            Exiled.Events.Handlers.Player.Left -= OnLeft;
            Exiled.Events.Handlers.Player.Died -= OnDied;
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;

            manager.Save();
        }

        private bool ShouldTrack(Player p)
        {
            return !(Config.RespectDNT && p.DoNotTrack);
        }

        private void OnJoin(VerifiedEventArgs ev)
        {
            if (!ShouldTrack(ev.Player)) return;

            string id = manager.GetId(ev.Player);

            if (!manager.Stats.ContainsKey(id))
                manager.Stats[id] = new PlayerStats();

            manager.Stats[id].Nickname = ev.Player.Nickname;
            manager.Stats[id].LastJoin = DateTime.Now;

            SupabaseSender.Send(id, manager.Stats[id], Config);
        }

        private void OnLeft(LeftEventArgs ev)
        {
            if (!ShouldTrack(ev.Player)) return;

            string id = manager.GetId(ev.Player);

            if (manager.Stats.TryGetValue(id, out var ps))
            {
                ps.TotalPlaySeconds += (int)(DateTime.Now - ps.LastJoin).TotalSeconds;
                ps.LastSeenUtc = DateTime.UtcNow;

                SupabaseSender.Send(id, ps, Config);
            }

            manager.Save();
        }

        private void OnDied(DiedEventArgs ev)
        {
            if (ShouldTrack(ev.Player))
            {
                string victimId = manager.GetId(ev.Player);

                if (!manager.Stats.ContainsKey(victimId))
                    manager.Stats[victimId] = new PlayerStats();

                manager.Stats[victimId].Deaths++;
                manager.Stats[victimId].LastSeenUtc = DateTime.UtcNow;

                SupabaseSender.Send(victimId, manager.Stats[victimId], Config);
            }

            if (ev.Attacker != null && ev.Attacker != ev.Player && ShouldTrack(ev.Attacker))
            {
                string killerId = manager.GetId(ev.Attacker);

                if (!manager.Stats.ContainsKey(killerId))
                    manager.Stats[killerId] = new PlayerStats();

                manager.Stats[killerId].Kills++;
                manager.Stats[killerId].LastSeenUtc = DateTime.UtcNow;

                SupabaseSender.Send(killerId, manager.Stats[killerId], Config);
            }

            manager.Save();
        }

        private void OnRoundEnded(RoundEndedEventArgs ev)
        {
            manager.Save();

            foreach (var kvp in manager.Stats)
                SupabaseSender.Send(kvp.Key, kvp.Value, Config);
        }
    }
}
