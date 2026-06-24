using UnityEngine;

namespace Gameplay.Entities.Enemies
{
    [CreateAssetMenu(menuName = "Magical Tower/Enemy Config")]
    public class EnemyConfig : ScriptableObject
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField] private int _maxHealth = 30;
        [SerializeField] private float _moveSpeed = 2f;
        [SerializeField] private float _attackDamage = 5f;
        [SerializeField] private float _attackInterval = 1f;
        [SerializeField] private float _attackRange = 1.25f;
        [SerializeField] private float _spawnWeight = 1f;

        public GameObject Prefab => _prefab;
        public int MaxHealth => _maxHealth;
        public float MoveSpeed => _moveSpeed;
        public float AttackDamage => _attackDamage;
        public float AttackInterval => _attackInterval;
        public float AttackRange => _attackRange;
        public float SpawnWeight => _spawnWeight;
    }
}
