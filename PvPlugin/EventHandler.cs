using CommandSystem.Commands.RemoteAdmin;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Firearms.Ammo;
using MEC;
using PlayerRoles;
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
            Exiled.Events.Handlers.Player.ChangingRole += OnChangingRole;
        }
        public void UnregisterEvents()
        {
            Exiled.Events.Handlers.Player.Verified -= OnVerified;
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
            Exiled.Events.Handlers.Player.ChangingRole -= OnChangingRole;
        }

        public void OnVerified(VerifiedEventArgs ev)
        {
            if (Round.InProgress == true)
            {
                ev.Player.Role.Set(RoleTypeId.Spectator);
            }
            else
            {
                ev.Player.Role.Set(RoleTypeId.Tutorial);
            }
        }

        public void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (ev.Player == null)
            {
                Log.Info($"{ev.Player.Nickname} このプレイヤーはnullです");
                return;
            }

            ev.Items.Clear();
            ev.Items.Add(ItemType.GunE11SR);
            ev.Items.Add(ItemType.ArmorCombat);
            ev.Ammo.Add(ItemType.Ammo556x45, 120);
        }

        public void OnRoundStarted()
        {
            Log.Info("ラウンドが開始されました！");
            Timing.RunCoroutine(RoundRoutine());
            Timing.RunCoroutine(RoundStart());
            Timing.RunCoroutine(BroadcastRemainingTime(237));
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

        private IEnumerator<float> BroadcastRemainingTime(int totalSeconds)
        {
            while (totalSeconds >= 0)
            {
                int minutes = totalSeconds / 60;
                int seconds = totalSeconds % 60;
                string message = $"<b><color=#00d9ff>--------------------\\</color></b> {minutes}:{(seconds < 10 ? "0" : "")}{seconds} <b><color=#00d9ff>/--------------------</color></b>";
                foreach (Player player in Player.List)
                {
                    player.ShowHint($"{message}" + "\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n"); // 1秒間メッセージを表示
                }
                yield return Timing.WaitForSeconds(1);
                totalSeconds--;
            }

            yield break; // コルーチン終了
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