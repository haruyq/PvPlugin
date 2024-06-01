using CommandSystem.Commands.RemoteAdmin;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using Interactables.Interobjects.DoorUtils;
using Interactables.Interobjects;
using InventorySystem.Items.Firearms.Ammo;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using static Interactables.Interobjects.ElevatorManager;
using Exiled.API.Features.Doors;
using System.Collections;
using ElevatorDoor = Exiled.API.Features.Doors.ElevatorDoor;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

namespace PvPluginEventHandler
{
    public class EventHandler : MonoBehaviour
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
            ev.Ammo.Clear();
            ev.Items.Add(ItemType.GunE11SR);
            ev.Items.Add(ItemType.ArmorCombat);
            ev.Ammo.Add(ItemType.Ammo556x45, 120);
        }

        public void OnRoundStarted()
        {
            Log.Info("ラウンドが開始されました！");
            Round.IsLocked = true;
            StartCoroutine(RoundRoutine());
            StartCoroutine(RoundStart());
            StartCoroutine(LockElevators());
            StartCoroutine(BroadcastRemainingTime(237));
        }

        private IEnumerator<float> RoundRoutine()
        {
            if (Round.InProgress == false)
            {
                Log.Info("ラウンド終了");
                StartCoroutine(RoundIsEnd());
            }
            else if (Round.InProgress == true)
            {
                yield return Timing.WaitForSeconds(240); // 240秒待機
                Log.Info("ラウンド終了");
                StartCoroutine(RoundIsEnd());
            }
        }

        private IEnumerator<float> LockElevators()
        {
            foreach (Door door in Door.List.Where(x => x.Type == DoorType.ElevatorGateA || x.Type == DoorType.ElevatorGateB))
                door.ChangeLock(DoorLockType.Warhead);
            Log.Info("エレベーターをロックしました");
            yield break;
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
            for (int i = 0; i < 5; i++)
            {
                StopCoroutine(RoundRoutine());
                StopCoroutine(RoundStart());
                StopCoroutine(LockElevators());
                StopCoroutine(BroadcastRemainingTime(0));

                yield return Timing.WaitForSeconds(10);

                Timing.WaitForSeconds(1);

                Log.Info("ラウンドが開始されました！");

                StartCoroutine(RoundRoutine());
                StartCoroutine(RoundStart());
                StartCoroutine(LockElevators());
                StartCoroutine(BroadcastRemainingTime(237));
            }
            yield break;
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
                player.Role.Set(RoleTypeId.NtfSergeant); // 例：役割をNTFに設定
            }

            foreach (Player player in secondHalf)
            {
                player.Role.Set(RoleTypeId.ChaosRifleman); // 例：役割をCIに設定
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