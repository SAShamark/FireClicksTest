using Entities;
using Gameplay.GUIWidgets;
using Services.ObjectPool;
using UnityEngine;

namespace Gameplay.Entities.Enemies
{
    public class Enemy : BasePoolDestroyable, IDamageable
    {
        [SerializeField] private SphereCollider _attackRangeCollider;
        [SerializeField] private Color _attackRangeColor = new Color(1f, 0.25f, 0.15f, 0.35f);
        [SerializeField] private string _towerTag = "Tower";
        [SerializeField] private float _aimPointHeight = 0.5f;
        [SerializeField] private float _moveStopDistanceSqr = 0.001f;
        [SerializeField] private float _burnTickInterval = 0.25f;

        private EnemyConfig _config;
        private Transform _target;
        private IDamageable _attackTarget;
        private float _attackTimer;
        private float _burnDamagePerSecond;
        private float _burnTimeLeft;
        private float _burnTickAccumulator;

        public float Health { get; private set; }
        public bool IsAlive => gameObject.activeInHierarchy && Health > 0f;
        public Vector3 AimPosition => transform.position + Vector3.up * _aimPointHeight;

        private void Update()
        {
            if (!IsAlive || _target == null)
            {
                return;
            }

            UpdateBurn();
            _attackTimer -= Time.deltaTime;

            if (_attackTarget != null)
            {
                AttackTarget();
                return;
            }

            MoveToTarget();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsTower(other.gameObject))
            {
                return;
            }

            _attackTarget = other.GetComponentInParent<IDamageable>();
        }

        private void OnTriggerExit(Collider other)
        {
            if (IsTower(other.gameObject))
            {
                _attackTarget = null;
            }
        }

        public void Configure(EnemyConfig config, Transform target)
        {
            _config = config;
            _target = target;
            Health = config.MaxHealth;
            EnsureAttackRangeCollider();
            _attackRangeCollider.radius = config.AttackRange;

            _attackTimer = 0f;
            _attackTarget = null;
            _burnDamagePerSecond = 0f;
            _burnTimeLeft = 0f;
            _burnTickAccumulator = 0f;
        }

        public void TakeDamage(float amount)
        {
            if (!IsAlive)
            {
                return;
            }

            Health = Mathf.Max(0f, Health - amount);
            FloatingNumbersGUI.Instance?.Spawn(AimPosition, amount, Color.white);

            if (Health <= 0f)
            {
                DestroyObject();
            }
        }

        public void ApplyBurn(float damagePerSecond, float duration)
        {
            _burnDamagePerSecond = Mathf.Max(_burnDamagePerSecond, damagePerSecond);
            _burnTimeLeft = Mathf.Max(_burnTimeLeft, duration);
        }

        private void MoveToTarget()
        {
            Vector3 targetPosition = _target.transform.position;
            Vector3 toTarget = targetPosition - transform.position;
            toTarget.y = 0f;

            if (toTarget.sqrMagnitude <= _moveStopDistanceSqr)
            {
                return;
            }

            Vector3 direction = toTarget.normalized;
            transform.position += direction * (_config.MoveSpeed * Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }

        private void AttackTarget()
        {
            if (_attackTimer > 0f)
            {
                return;
            }

            _attackTarget.TakeDamage(_config.AttackDamage);
            _attackTimer = _config.AttackInterval;
        }

        private bool IsTower(GameObject hitObject)
        {
            Transform current = hitObject.transform;
            while (current != null)
            {
                if (current.gameObject.CompareTag(_towerTag))
                {
                    return true;
                }

                current = current.parent;
            }

            return false;
        }

        private void EnsureAttackRangeCollider()
        {
            if (_attackRangeCollider == null)
            {
                _attackRangeCollider = gameObject.AddComponent<SphereCollider>();
            }

            _attackRangeCollider.isTrigger = true;
            _attackRangeCollider.center = Vector3.zero;

            if (!TryGetComponent(out Rigidbody rigidbody))
            {
                rigidbody = gameObject.AddComponent<Rigidbody>();
            }

            rigidbody.isKinematic = true;
            rigidbody.useGravity = false;
        }

        private void UpdateBurn()
        {
            if (_burnTimeLeft <= 0f)
            {
                return;
            }

            float deltaTime = Time.deltaTime;
            _burnTimeLeft -= deltaTime;
            _burnTickAccumulator += deltaTime;

            if (_burnTickAccumulator >= _burnTickInterval)
            {
                float damage = _burnDamagePerSecond * _burnTickAccumulator;
                _burnTickAccumulator = 0f;
                TakeDamage(damage);
            }
        }
    }
}
