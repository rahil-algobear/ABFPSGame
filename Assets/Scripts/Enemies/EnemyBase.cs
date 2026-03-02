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
        [SerializeField] protected int _scoreValue = 100;

        protected float _currentHealth;
        protected bool _isAlive = true;

        public bool IsAlive => _isAlive;
        public float CurrentHealth => _currentHealth;
        public float MaxHealth => _maxHealth;
        #endregion

        #region Unity Lifecycle
        protected virtual void Awake()
        {
            _currentHealth = _maxHealth;
        }

        protected virtual void Start()
        {
        }

        protected virtual void Update()
        {
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

            // Register kill with GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RegisterEnemyKill(_scoreValue);
            }

            // Cleanup
            Destroy(gameObject, 2f);
        }
        #endregion

        #region Abstract Methods
        /// <summary>
        /// Perform attack on target.
        /// </summary>
        /// <param name="target">Target transform</param>
        protected virtual void PerformAttack(Transform target)
        {
            // Base implementation - can be overridden by subclasses
        }
        #endregion
    }
}
