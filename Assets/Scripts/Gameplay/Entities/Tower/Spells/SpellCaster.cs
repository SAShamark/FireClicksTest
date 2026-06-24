using System.Collections.Generic;
using Gameplay.Entities.Enemies;
using Services.ObjectPool;
using UnityEngine;

namespace Gameplay.Entities.Tower.Spells
{
    public class SpellCaster
    {
        private readonly Dictionary<SpellConfig, ObjectPool> _pools = new();
        private readonly Dictionary<SpellConfig, float> _cooldowns = new();
        private readonly Dictionary<SpellConfig, BaseSpell> _spellBehaviours = new();
        private EnemyRegistry _enemyRegistry;
        private Transform _projectileRoot;
        private Camera _camera;
        private List<SpellConfig> _spellConfigs;
        private Transform _spawnPoint;
        private int _poolPrewarmCount;

        public void Initialize(List<SpellConfig> spellConfigs, Transform spawnPoint, EnemyRegistry enemyRegistry,
            int poolPrewarmCount)
        {
            _spellConfigs = spellConfigs;
            _spawnPoint = spawnPoint;
            _enemyRegistry = enemyRegistry;
            _poolPrewarmCount = poolPrewarmCount;
            _camera = Camera.main;
            CreatePools();
        }

        public void Update()
        {
            foreach (var spell in _spellConfigs)
            {
                _cooldowns.TryAdd(spell, 0f);

                float cooldown = _cooldowns[spell] - Time.deltaTime;
                _cooldowns[spell] = cooldown;

                if (cooldown <= 0f && TryCast(spell))
                {
                    _cooldowns[spell] = spell.Cooldown;
                }
            }
        }

        private void CreatePools()
        {
            _pools.Clear();
            _cooldowns.Clear();
            _spellBehaviours.Clear();

            foreach (var spell in _spellConfigs)
            {
                _cooldowns.Add(spell, 0f);

                if (!spell.Prefab.TryGetComponent(out BaseSpell spellBehaviour))
                {
                    Debug.LogError($"{spell.name} prefab must have a {nameof(BaseSpell)} component.", spell.Prefab);
                    continue;
                }

                _spellBehaviours.Add(spell, spellBehaviour);

                if (!_pools.ContainsKey(spell))
                {
                    _pools.Add(spell, new ObjectPool(spell.Prefab, Mathf.Max(0, _poolPrewarmCount), _projectileRoot));
                }
            }
        }

        private bool TryCast(SpellConfig spell)
        {
            if (!_pools.TryGetValue(spell, out ObjectPool pool) ||
                !_spellBehaviours.TryGetValue(spell, out BaseSpell spellBehaviour))
            {
                return false;
            }

            return spellBehaviour.TryCast(spell, pool, _spawnPoint, _enemyRegistry, _camera);
        }
    }
}
