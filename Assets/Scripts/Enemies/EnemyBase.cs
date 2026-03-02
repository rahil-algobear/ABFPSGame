using UnityEngine;
using Game.Core;

namespace Game.Enemies
{
    public abstract class EnemyBase : MonoBehaviour, IDamageable
    {
        [Header("Stats")]
        [SerializeField] protected float _maxHealth = 100f;
        [SerializeField] protected float _currentHealth;
        [SerializeField] protected float _moveSpeed = 3f;
        [SerializeField] protected float _attackDamage = 10f;
        [SerializeField] protected float _attackRange = 2f;
        [SerializeField] protected float _attackCooldown = 1f;

        [Header("Detection")]
        [SerializeField] protected float _detectionRange = 15f;
        [SerializeField] protected LayerMask _playerMask;

        protected Transform _player;
        protected float _lastAttackTime;
        protected bool _isAlive = true;

        public bool IsAlive => _isAlive;
        public bool IsDead => !_isAlive;

        protected virtual void Awake()
        {
            _currentHealth = _maxHealth;
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                _player = playerObj.transform;
            }
        }

        public virtual void TakeDamage(float damage, DamageSystem.DamageType damageType, Vector3 hitPoint)
        {
            if (!_isAlive) return;

            _currentHealth -= damage;

            if (_currentHealth <= 0)
            {
                Die();
            }
        }

        public virtual void Die()
        {
            if (!_isAlive) return;

            _isAlive = false;
            Game.Core.GameManager.Instance.RegisterEnemyKill(100);
            Destroy(gameObject, 2f);
        }

        protected virtual void Attack()
        {
            if (_player == null || Time.time < _lastAttackTime + _attackCooldown)
                return;

            float distanceToPlayer = Vector3.Distance(transform.position, _player.position);
            if (distanceToPlayer <= _attackRange)
            {
                _lastAttackTime = Time.time;
                var playerHealth = _player.GetComponent<Game.Player.PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(_attackDamage, DamageSystem.DamageType.Melee, transform.position);
                }
            }
        }
    }
}