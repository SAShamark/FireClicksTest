using System;
using Entities;
using Gameplay.Entities.Enemies;
using Gameplay.Entities.Tower.Spells;
using Gameplay.GUIWidgets;
using UnityEngine;

namespace Gameplay.Entities.Tower
{
    public class TowerControl : MonoBehaviour, IDamageable
    {
        [SerializeField] private Transform _projectileSpawnPoint;
        [SerializeField] private int _spellPoolPrewarmCount = 3;
        [SerializeField] private float _damageNumberHeight = 2.5f;
        [SerializeField] private Color _damageNumberColor = Color.red;

        private readonly SpellCaster _spellCaster = new();
        private bool _isDead;

        public float Health { get; private set; }
        public float MaxHealth { get; private set; }
        
        public event Action<float> OnHealthChanged;
        public event Action OnDied;

        public void Initialize(TowerConfig config, EnemyRegistry enemyRegistry)
        {
            MaxHealth = config.MaxHealth;
            Health = MaxHealth;
            _isDead = false;
            _spellCaster.Initialize(config.Spells, _projectileSpawnPoint, enemyRegistry, _spellPoolPrewarmCount);
            OnHealthChanged?.Invoke(Health);
        }

        private void Update()
        {
            _spellCaster.Update();
        }

        public void TakeDamage(float amount)
        {
            if (_isDead)
            {
                return;
            }

            Health = Mathf.Max(0, Health - amount);
            FloatingNumbersGUI.Instance?.Spawn(transform.position + Vector3.up * _damageNumberHeight, amount,
                _damageNumberColor);
            OnHealthChanged?.Invoke(Health);

            if (Health <= 0)
            {
                _isDead = true;
                OnDied?.Invoke();
            }
        }
    }
}