using UnityEngine;

namespace Gameplay.Entities.Tower.Spells
{
    [CreateAssetMenu(menuName = "Magical Tower/Spells/Fireball")]
    public class FireballSpellConfig : SpellConfig
    {
        [SerializeField] private float _explosionRadius = 2.5f;
        [SerializeField] private float _burnDamagePerSecond = 5f;
        [SerializeField] private float _burnDuration = 3f;

        public float ExplosionRadius => _explosionRadius;
        public float BurnDamagePerSecond => _burnDamagePerSecond;
        public float BurnDuration => _burnDuration;
    }
}
