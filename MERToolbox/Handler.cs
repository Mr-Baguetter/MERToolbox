using System.Collections.Generic;
using InventorySystem.Items.Firearms.Modules;
using InventorySystem.Items.ThrowableProjectiles;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using NorthwoodLib.Pools;
using MERToolbox.API.Components;
using UnityEngine;
using PlayerEvent = LabApi.Events.Handlers.PlayerEvents;
using ServerEvent = LabApi.Events.Handlers.ServerEvents;
using MERToolbox.Patches;
using PlayerRoles.PlayableScps.Scp939;
using LabApi.Features.Wrappers;
using PlayerRoles.PlayableScps.Scp096;
using MERToolbox.API.Enums;
using MERToolbox.API.Helpers;

namespace MERToolbox
{
    public static class Handler
    {
        internal static readonly CachedLayerMask Mask = new("Default", "Door", "Glass");

        public static void Register()
        {
            PlayerEvent.ShotWeapon += OnPlayerShotWeapon;
            Scp939ClawAbilityPrefix.OnClawAttempted += OnScp939Attacked;
            Scp096AttackAbilityPostfix.OnSwingTriggered += OnScp096Attacked;
            ServerEvent.ExplosionSpawned += OnExplosionSpawned;
            ServerEvent.RoundStarting += OnRoundStarting;
        }

        public static void Unregister()
        {
            PlayerEvent.ShotWeapon -= OnPlayerShotWeapon;
            Scp939ClawAbilityPrefix.OnClawAttempted -= OnScp939Attacked;
            Scp096AttackAbilityPostfix.OnSwingTriggered -= OnScp096Attacked;
            ServerEvent.ExplosionSpawned -= OnExplosionSpawned;
            ServerEvent.RoundStarting -= OnRoundStarting;
        }

        private static void OnRoundStarting(RoundStartingEventArgs ev)
        {
            PrefabManager.RegisterPrefabs();
            MERNukePatchFix.UnpatchOriginal();
        }
        
        private static void OnScp096Attacked(Scp096AttackAbility ev)
        {
            if (Physics.Raycast(ev.Owner.PlayerCameraReference.position + ev.Owner.PlayerCameraReference.forward, ev.Owner.PlayerCameraReference.forward, out RaycastHit hitInfo, 5, Mask))
            {
                FluidTank tank = hitInfo.collider.GetComponentInParent<FluidTank>();
                if (tank != null && tank.TryDamaging(DamageTypes.Scp096, Scp096AttackAbility.HumanDamage))
                    Player.Get(ev.Owner).SendHitMarker(Scp096AttackAbility.HumanDamage);
            }
        }

        private static void OnScp939Attacked(Scp939ClawAbility ev)
        {
            if (Physics.Raycast(ev.Owner.PlayerCameraReference.position + ev.Owner.PlayerCameraReference.forward, ev.Owner.PlayerCameraReference.forward, out RaycastHit hitInfo, 5, Mask))
            {
                FluidTank tank = hitInfo.collider.GetComponentInParent<FluidTank>();
                if (tank != null && tank.TryDamaging(DamageTypes.Scp939, ev.DamageAmount))
                    Player.Get(ev.Owner).SendHitMarker(ev.DamageAmount);
            }
        }

        private static void OnPlayerShotWeapon(PlayerShotWeaponEventArgs ev)
        {
            if (ev.FirearmItem.Base.TryGetModule<HitscanHitregModuleBase>(out var hitscanreg))
            {
                if (Physics.Raycast(ev.Player.Camera.position + ev.Player.Camera.forward, ev.Player.Camera.forward, out RaycastHit hitInfo, hitscanreg.DamageFalloffDistance + hitscanreg.FullDamageDistance, Mask))
                {
                    FluidTank tank = hitInfo.collider.GetComponentInParent<FluidTank>();
                    if (tank != null && tank.TryDamaging(DamageTypes.Weapon, 10))
                        ev.Player.SendHitMarker(10);
                }
            }
            if (ev.FirearmItem.Base.TryGetModule<DisruptorHitregModule>(out var disruptorhitscanreg))
            {
                if (Physics.Raycast(ev.Player.Camera.position + ev.Player.Camera.forward, ev.Player.Camera.forward, out RaycastHit hitInfo, disruptorhitscanreg.DamageFalloffDistance + disruptorhitscanreg.FullDamageDistance, Mask))
                {
                    FluidTank tank = hitInfo.collider.GetComponentInParent<FluidTank>();
                    if (tank != null && tank.TryDamaging(DamageTypes.ParticleDisruptor, 200))
                        ev.Player.SendHitMarker(200);
                }
            }
        }

        private static void OnExplosionSpawned(ExplosionSpawnedEventArgs ev)
        {
            Collider[] colliders = Physics.OverlapSphere(ev.Position, ev.Settings.MaxRadius, ev.Settings.DetectionMask, QueryTriggerInteraction.Ignore);
            if (colliders == null || colliders.Length == 0)
                return;
                
            HashSet<FluidTank> tanksSeen = HashSetPool<FluidTank>.Shared.Rent();
            try
            {
                foreach (Collider col in colliders)
                {
                    if (col == null) 
                        continue;

                    FluidTank tank = col.GetComponentInParent<FluidTank>();
                    if (tank == null)
                        continue;

                    if (!tanksSeen.Add(tank)) 
                        continue;

                    Vector3 tankPos = tank.transform.position;
                    float distance = Vector3.Distance(ev.Position, tankPos);
                    if (distance > ev.Settings.MaxRadius) 
                        continue;

                    if (Physics.Linecast(tankPos, ev.Position, (int)ThrownProjectile.HitBlockerMask))
                        continue;

                    if (tank != null && tank.TryDamaging(DamageTypes.Explosion, ev.Settings._doorDamageOverDistance.Evaluate(distance)))
                        ev.Player?.SendHitMarker(ev.Settings._doorDamageOverDistance.Evaluate(distance));
                }
            }
            finally
            {
                HashSetPool<FluidTank>.Shared.Return(tanksSeen);
            }
        }
    }
}