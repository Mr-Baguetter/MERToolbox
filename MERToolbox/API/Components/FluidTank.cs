using InventorySystem;
using InventorySystem.Items.ThrowableProjectiles;
using LabApi.Features.Wrappers;
using MEC;
using Mirror;
using MERToolbox.API.Data;
using MERToolbox.API.Enums;
using UnityEngine;
using static MERToolbox.API.Extensions.AnimationCurveExtensions;

namespace MERToolbox.API.Components
{
    public class FluidTank : MonoBehaviour
    {
        public PrimitiveObjectToy Primitive;
        public TankData Data;
        public float CurrentHealth;
        public Player ShotPlayer;
        private bool _alreadyDetonated;

        public void Init(PrimitiveObjectToy primitive, TankData data)
        {
            Primitive = primitive;
            Data = data;
            CurrentHealth = Data.Health;
        }

        private void ApplyDamage(float damage)
        {
            if (damage <= 0f)
                return;

            if (CurrentHealth <= 0)
                return;

            CurrentHealth -= damage;
        }

        public bool TryDamaging(DamageTypes damageType, float damage)
        {
            if (Data.AllowedDamageTypes.Contains(damageType))
            {
                ApplyDamage(damage);
                return true;
            }

            return false;
        }

        private void Update()
        {
            if (CurrentHealth <= 10 && !_alreadyDetonated)
            {
                if (!InventoryItemLoader.TryGetItem(ItemType.GrenadeHE, out InventorySystem.Items.ThrowableProjectiles.ThrowableItem result))
                    return;

                if (result.Projectile is not TimeGrenade prefab)
                    return;

                TimeGrenade timeGrenade = Instantiate(prefab, Primitive.Position, Quaternion.identity);

                if (!timeGrenade.TryGetComponent(out TimedGrenadeProjectile projectile))
                    return;

                if (projectile is ExplosiveGrenadeProjectile grenade)
                {
                    grenade.Base._playerDamageOverDistance.Multiply(Data.ExplosionPlayerDamageMultiplier);
                    grenade.Base._doorDamageOverDistance.Multiply(Data.ExplosionDoorDamageMultiplier);
                }

                projectile.RemainingTime = 1f;
                NetworkServer.Spawn(timeGrenade.gameObject);
                timeGrenade.ServerActivate();
                Timing.CallDelayed(1f, () => Primitive.Destroy());

                _alreadyDetonated = true;
            }
        }

    }
}