using System;
using System.Collections.Generic;
using Services.ObjectPool;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.Entities.Enemies
{
    [Serializable]
    public class EnemySpawnManager
    {
        [SerializeField] private Transform _enemyRoot;
        [SerializeField] private List<Transform> _spawnPoints = new();
        [SerializeField] private float _spawnAccumulator;
        [SerializeField] private float _spawnAccumulatorThreshold = 1f;
        [SerializeField] private int _poolPrewarmCount = 4;

        private readonly Dictionary<EnemyConfig, ObjectPool> _pools = new();
        private List<EnemyConfig> _enemyConfigs;
        private GameplayPhaseConfig _gameplayPhaseConfig;
        private Transform _target;
        private float _elapsedTime;

        public EnemyRegistry EnemyRegistry { get; } = new();

        public void Update(float deltaTime)
        {
            _elapsedTime += deltaTime;
            GameplayPhaseConfig.SpawnPhase phase = _gameplayPhaseConfig.GetSpawnPhase(_elapsedTime);
            _spawnAccumulator += deltaTime * Mathf.Max(0f, phase.SpawnsPerSecond);
            float spawnAccumulatorThreshold = Mathf.Max(float.Epsilon, _spawnAccumulatorThreshold);

            while (_spawnAccumulator >= spawnAccumulatorThreshold)
            {
                SpawnEnemy();
                _spawnAccumulator -= spawnAccumulatorThreshold;
            }
        }

        public void Initialize(List<EnemyConfig> enemyConfigs, GameplayPhaseConfig gameplayPhaseConfig, Transform target)
        {
            _enemyConfigs = enemyConfigs;
            _gameplayPhaseConfig = gameplayPhaseConfig;
            _target = target;
            _elapsedTime = 0f;
            _spawnAccumulator = 0f;
            
            CreatePools();
        }

        private void CreatePools()
        {
            _pools.Clear();
            foreach (var config in _enemyConfigs)
            {
                if (!_pools.ContainsKey(config))
                {
                    _pools.Add(config, new ObjectPool(config.Prefab, Mathf.Max(0, _poolPrewarmCount), _enemyRoot));
                }
            }
        }

        private void SpawnEnemy()
        {
            EnemyConfig config = PickEnemyConfig();
            if (config == null || !_pools.TryGetValue(config, out ObjectPool pool))
            {
                return;
            }

            GameObject enemyObject = pool.GetFreeElement();
            Vector3 spawnPosition = PickSpawnPosition();
            enemyObject.transform.position = spawnPosition;

            Enemy enemy = enemyObject.GetComponent<Enemy>();
            if (enemy == null)
            {
                enemy = enemyObject.AddComponent<Enemy>();
                enemy.Init(pool);
            }

            enemy.Configure(config, _target);
            EnemyRegistry.Register(enemy);
        }

        private Vector3 PickSpawnPosition()
        {
            return _spawnPoints[Random.Range(0, _spawnPoints.Count)].position;
        }

        private EnemyConfig PickEnemyConfig()
        {
            float totalWeight = 0f;
            foreach (var config in _enemyConfigs)
            {
                totalWeight += Mathf.Max(0f, config.SpawnWeight);
            }

            if (totalWeight <= 0f)
            {
                return null;
            }

            float roll = Random.Range(0f, totalWeight);
            foreach (var config in _enemyConfigs)
            {
                roll -= Mathf.Max(0f, config.SpawnWeight);
                if (roll <= 0f)
                {
                    return config;
                }
            }

            return _enemyConfigs[^1];
        }
    }
}
