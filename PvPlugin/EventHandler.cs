using Exiled.API.Features;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using MEC;
using System;
using System.Collections.Generic;

namespace PvPluginEventHandler
{
    public class EventHandler
    {
		public static bool IsEventEnabled { get; set; } = false;

        public void RegisterEvents()
        {
            Exiled.Events.Handlers.Player.Verified += OnVerified;
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
        }
        public void UnregisterEvents()
        {
            Exiled.Events.Handlers.Player.Verified -= OnVerified;
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
        }

        public void OnVerified(VerifiedEventArgs ev)
        {
            if (Round.InProgress == true)
            {
                ev.Player.Role.Set(PlayerRoles.RoleTypeId.Spectator);
            }
            else
            {
                ev.Player.Role.Set(PlayerRoles.RoleTypeId.Tutorial);
            }
        }

        public void OnRoundStarted()
        {
            Log.Info("ラウンドが開始されました！");
            Timing.RunCoroutine(RoundRoutine());
            Timing.RunCoroutine(RoundStart());
        }

        private IEnumerator<float> RoundRoutine()
        {
            if (Round.InProgress == false)
            {
                Log.Info("ラウンド終了");
                Timing.RunCoroutine(RoundIsEnd());
            }
            else if (Round.InProgress == true)
            {
                yield return Timing.WaitForSeconds(240); // 240秒待機
                Log.Info("ラウンド終了");
                Timing.RunCoroutine(RoundIsEnd());
            }
        }

        private IEnumerator<float> RoundIsEnd()
        {
            foreach (Player player in Player.List)
            {
                player.Role.Set(PlayerRoles.RoleTypeId.Tutorial); // 全プレイヤーの役割をチュートリアルに設定
            }

            yield return Timing.WaitForSeconds(20); // さらに20秒待機
            Round.Restart(); // ラウンドを再スタート
        }

        private IEnumerator<float> RoundStart()
        {
            // プレイヤーリストを取得
            List<Player> players = new List<Player>(Player.List);

            // プレイヤーリストをシャッフルしてランダム化
            players.Shuffle();

            // プレイヤーリストを半分に分割
            int halfCount = players.Count / 2;
            List<Player> firstHalf = players.GetRange(0, halfCount);
            List<Player> secondHalf = players.GetRange(halfCount, players.Count - halfCount);

            // ここで各グループに対して処理を行う
            foreach (Player player in firstHalf)
            {
                player.Role.Set(PlayerRoles.RoleTypeId.NtfSergeant); // 例：役割をNTFに設定
            }

            foreach (Player player in secondHalf)
            {
                player.Role.Set(PlayerRoles.RoleTypeId.ChaosRifleman); // 例：役割をCIに設定
            }

            yield break; // コルーチン終了
        }
    }

    public static class Extensions
    {
        private static System.Random rng = new System.Random();

        // リストをシャッフルする拡張メソッド
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}