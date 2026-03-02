using UnityEngine;
using Game.Core;

namespace Game.Enemies
{
    /// <summary>
    /// Base class for all enemy types.
    /// </summary>
    public abstract class EnemyBase : MonoBehaviour, IDamageable
    {
        #region Properties
        [Header("Enemy Stats")]
        [SerializeField] protected float _maxHealth = 100f;
        [SerializeField] protected float _moveSpeed = 3f;
        [SerializeField] protected float _attackDamage = 10f;
        [SerializeField] protected float _attackRange = 2f;
        [SerializeField] protected float _attackCooldown = 1f;

        protected float _currentHealth;
        protected float _lastAttackTime;
        protected bool _isAlive = true;

        public bool IsAlive => _isAlive;
        public float CurrentHealth => _currentHealth;
        public float MaxHealth => _maxHealth;
        #endregion

        #region Virtual Methods
        /// <summary>
        /// Virtual Awake method that can be overridden by subclasses.
        /// </summary>
        protected virtual void Awake()
        {
            _currentHealth = _maxHealth;
        }

        /// <summary>
        /// Virtual Start method that can be overridden by subclasses.
        /// </summary>
        protected virtual void Start()
        {
            // Base implementation - can be overridden
        }

        /// <summary>
        /// Virtual method for performing attacks. Must be implemented by subclasses.
        /// </summary>
        /// <param name="target">The target to attack</param>
        protected virtual void PerformAttack(Transform target)
        {
            // Base implementation - can be overridden
        }
        #endregion

        #region IDamageable Implementation
        public virtual void TakeDamage(float damage, DamageSystem.DamageType damageType, Vector3 hitPoint)
        {
            if (!_isAlive) return;

            _currentHealth -= damage;

            if (_currentHealth <= 0f)
            {
                Die();
            }
        }

        public virtual void Die()
        {
            if (!_isAlive) return;

            _isAlive = false;
            // Notify GameManager of enemy death
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RegisterEnemyKill(100);
            }
        }
        #endregion
    }
}
