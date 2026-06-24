using UnityEngine;

namespace Gameplay.Entities.Tower.Spells
{
    public abstract class SpellConfig : ScriptableObject
    {
        [SerializeField] private float _cooldown = 1f;
        [SerializeField] private float _damage = 10f;
        [SerializeField] private float _projectileSpeed = 10f;
        [SerializeField] private GameObject _prefab;
        public float Cooldown => _cooldown;
        public float Damage => _damage;
        public float ProjectileSpeed => _projectileSpeed;
        public GameObject Prefab => _prefab;
    }
}
