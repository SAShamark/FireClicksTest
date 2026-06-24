using Gameplay.Entities.Enemies;
using DG.Tweening;
using Gameplay.GUIWidgets;
using Services.ObjectPool;
using UnityEngine;

namespace Gameplay.Entities.Tower.Spells
{
    public class FireballSpell : BaseSpell
    {
        [SerializeField] private float _hitDistance = 0.65f;
        [SerializeField] private float _maxTravelDistance = 28f;
        [SerializeField] private float _explosionVisualDuration = 0.28f;
        [SerializeField] private float _damageNumberHeight = 0.5f;
        [SerializeField] private Color _damageNumberColor = new Color(1f, 0.45f, 0.1f);
        [SerializeField] private float _explosionDiameterMultiplier = 2f;
        [SerializeField] private float _explosionExpandDurationMultiplier = 0.65f;
        [SerializeField] private float _explosionSettleDurationMultiplier = 0.35f;
        [SerializeField] private float _explosionOvershootScale = 1.08f;
        [SerializeField] private GameObject _explosionPrefab;

        private FireballSpellConfig _config;
        private EnemyRegistry _enemyRegistry;
        private Vector3 _direction;
        private Vector3 _startPosition;

        private void Update()
        {
            transform.position += _direction * (Speed * Time.deltaTime);

            if (Vector3.Distance(_startPosition, transform.position) >= _maxTravelDistance || TryHitEnemy())
            {
                Explode();
            }
        }

        public override bool TryCast(SpellConfig config, ObjectPool pool, Transform spawnPoint,
            EnemyRegistry enemyRegistry, Camera camera)
        {
            if (config is not FireballSpellConfig fireballConfig)
            {
                return false;
            }

            Enemy target = enemyRegistry.GetRandomEnemy();
            if (target == null)
            {
                return false;
            }

            Vector3 startPosition = spawnPoint.position;
            Vector3 direction = target.AimPosition - startPosition;

            FireballSpell projectile = pool.GetFreeElement().GetComponent<FireballSpell>();
            projectile.Launch(startPosition, direction, fireballConfig, enemyRegistry);
            return true;
        }

        public void Launch(Vector3 startPosition, Vector3 direction, FireballSpellConfig config,
            EnemyRegistry enemyRegistry)
        {
            _config = config;
            _enemyRegistry = enemyRegistry;
            _direction = direction.normalized;
            _startPosition = startPosition;
            transform.position = startPosition;
            transform.rotation = Quaternion.LookRotation(_direction, Vector3.up);
            SetSpeed(config.ProjectileSpeed);
        }

        private bool TryHitEnemy()
        {
            if (_enemyRegistry == null)
            {
                return false;
            }

            foreach (var enemy in _enemyRegistry.Enemies)
            {
                if (enemy.IsAlive && Vector3.Distance(transform.position, enemy.AimPosition) <= _hitDistance)
                {
                    return true;
                }
            }

            return false;
        }

        private void Explode()
        {
            Vector3 explosionPosition = transform.position;

            if (_enemyRegistry != null)
            {
                for (int i = _enemyRegistry.Enemies.Count - 1; i >= 0; i--)
                {
                    Enemy enemy = _enemyRegistry.Enemies[i];
                    if (enemy == null || !enemy.IsAlive)
                    {
                        continue;
                    }

                    if (Vector3.Distance(explosionPosition, enemy.transform.position) <= _config.ExplosionRadius)
                    {
                        enemy.TakeDamage(_config.Damage);
                        enemy.ApplyBurn(_config.BurnDamagePerSecond, _config.BurnDuration);
                    }
                }
            }

            SpawnExplosionVisual(explosionPosition);
            FloatingNumbersGUI.Instance?.Spawn(explosionPosition + Vector3.up * _damageNumberHeight, _config.Damage,
                _damageNumberColor);
            DestroyObject();
        }

        private void SpawnExplosionVisual(Vector3 position)
        {
            if (_explosionPrefab == null)
            {
                return;
            }

            GameObject explosion = Instantiate(_explosionPrefab);
            explosion.transform.position = position;
            explosion.transform.localScale = Vector3.zero;

            Collider[] colliders = explosion.GetComponentsInChildren<Collider>();
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].enabled = false;
            }

            float diameter = _config.ExplosionRadius * _explosionDiameterMultiplier;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(explosion.transform.DOScale(Vector3.one * diameter,
                _explosionVisualDuration * _explosionExpandDurationMultiplier).SetEase(Ease.OutCubic));
            sequence.Append(explosion.transform.DOScale(Vector3.one * (diameter * _explosionOvershootScale),
                _explosionVisualDuration * _explosionSettleDurationMultiplier).SetEase(Ease.InQuad));
            sequence.OnComplete(() => Destroy(explosion));
        }
    }
}
