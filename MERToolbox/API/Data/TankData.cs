using System.Collections.Generic;
using MERToolbox.API.Enums;

namespace MERToolbox.API.Data
{
    public class TankData
    {
        public string PrimitiveName { get; set; } = string.Empty;
        public float Health { get; set; } = 1000f;
        public float ExplosionPlayerDamageMultiplier { get; set; } = 5f;
        public float ExplosionDoorDamageMultiplier { get; set; } = 5f;
        public List<DamageTypes> AllowedDamageTypes { get; set; } =
        [
            DamageTypes.Explosion,
            DamageTypes.ParticleDisruptor,
            DamageTypes.Scp096,
            DamageTypes.Scp939,
            DamageTypes.Weapon
        ];
    }
}