using Gameplay.Entities.Enemies;
using Gameplay.GUIWidgets;
using Services.ObjectPool;
using UnityEngine;

namespace Gameplay.Entities.Tower.Spells
{
    public class BarrageSpell : BaseSpell
    {
        [SerializeField] private float _rotationThresholdSqr = 0.001f;
        [SerializeField] private float _completedProgress = 1f;
        [SerializeField] private float _fallbackDistance = 1f;
        [SerializeField] private float _minDuration = 0.1f;
        [SerializeField] private float _minProjectileSpeed = 0.1f;
        [SerializeField] private Color _damageNumberColor = new Color(0.35f, 0.8f, 1f);

        private BarrageSpellConfig _config;
        private Enemy _target;
        private Vector3 _startPosition;
        private float _duration;
        private float _elapsedTime;

        private void Update()
        {
            if (_target == null || !_target.IsAlive)
            {
                DestroyObject();
                return;
            }

            _elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(_elapsedTime / _duration);
            Vector3 targetPosition = _target.AimPosition;
            Vector3 position = Vector3.Lerp(_startPosition, targetPosition, progress);
            position.y += Mathf.Sin(progress * Mathf.PI) * _config.arcHeight;
            transform.position = position;

            Vector3 nextDirection = targetPosition - transform.position;
            if (nextDirection.sqrMagnitude > _rotationThresholdSqr)
            {
                transform.rotation = Quaternion.LookRotation(nextDirection.normalized, Vector3.up);
            }

            if (progress >= _completedProgress)
            {
                _target.TakeDamage(_config.Damage);
                FloatingNumbersGUI.Instance?.Spawn(targetPosition, _config.Damage, _damageNumberColor);
                DestroyObject();
            }
        }

        public override bool TryCast(SpellConfig config, ObjectPool pool, Transform spawnPoint,
            EnemyRegistry enemyRegistry, Camera camera)
        {
            if (config is not BarrageSpellConfig barrageConfig)
            {
                return false;
            }

            var targets = enemyRegistry.GetVisibleEnemies(camera);
            if (targets.Count == 0)
            {
                return false;
            }

            Vector3 startPosition = spawnPoint.position;
            foreach (var target in targets)
            {
                BarrageSpell projectile = pool.GetFreeElement().GetComponent<BarrageSpell>();
                projectile.Launch(startPosition, target, barrageConfig);
            }

            return true;
        }

        public void Launch(Vector3 startPosition, Enemy target, BarrageSpellConfig config)
        {
            _target = target;
            _config = config;
            _startPosition = startPosition;
            _elapsedTime = 0f;
            transform.position = startPosition;
            SetSpeed(config.ProjectileSpeed);

            float distance = target != null ? Vector3.Distance(startPosition, target.AimPosition) : _fallbackDistance;
            _duration = Mathf.Max(_minDuration, distance / Mathf.Max(_minProjectileSpeed, config.ProjectileSpeed));
        }
    }
}
