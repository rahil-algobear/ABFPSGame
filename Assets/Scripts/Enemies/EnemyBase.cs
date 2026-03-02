using UnityEngine;
using Game.Core;
using Game.Weapons;

namespace Game.Enemies
{
    /// <summary>
    /// Base class for all enemy types.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class EnemyBase : MonoBehaviour, IDamageable
    {
        #region Health
        [Header("Health")]
        [SerializeField] protected float _maxHealth = 100f;
        [SerializeField] protected float _currentHealth;

        public float MaxHealth => _maxHealth;
        public float CurrentHealth => _currentHealth;
        public bool IsAlive => _currentHealth > 0f;
        #endregion

        #region Damage Settings
        [Header("Damage Settings")]
        [SerializeField] protected float _meleeDamage = 10f;
        [SerializeField] protected float _attackRange = 2f;
        [SerializeField] protected float _attackCooldown = 1f;

        protected float _lastAttackTime;
        #endregion

        #region Score
        [Header("Score")]
        [SerializeField] protected int _scoreValue = 100;
        #endregion

        #region Components
        protected Rigidbody _rigidbody;
        protected Collider _collider;
        protected EnemyAI _ai;
        #endregion

        #region Unity Lifecycle
        protected virtual void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
            _ai = GetComponent<EnemyAI>();

            _currentHealth = _maxHealth;
        }
        #endregion

        #region IDamageable Implementation
        /// <summary>
        /// Take damage from a weapon hit.
        /// </summary>
        /// <param name="damage">Damage amount</param>
        /// <param name="hitPoint">Hit position</param>
        /// <param name="hitDirection">Hit direction</param>
        public virtual void TakeDamage(float damage, Vector3 hitPoint, Vector3 hitDirection)
        {
            if (!IsAlive) return;

            _currentHealth -= damage;

            // Alert AI
            if (_ai != null)
            {
                _ai.OnDamageTaken(hitPoint, hitDirection);
            }

            if (_currentHealth <= 0f)
            {
                Die();
            }
        }
        #endregion

        #region Death
        /// <summary>
        /// Handle enemy death.
        /// </summary>
        protected virtual void Die()
        {
            // Register kill
            GameManager.Instance.RegisterEnemyKill(_scoreValue);

            // Disable AI
            if (_ai != null)
            {
                _ai.enabled = false;
            }

            // Enable ragdoll or play death animation
            EnableRagdoll();

            // Destroy after delay
            Destroy(gameObject, 5f);
        }

        /// <summary>
        /// Enable ragdoll physics.
        /// </summary>
        protected virtual void EnableRagdoll()
        {
            if (_rigidbody != null)
            {
                _rigidbody.isKinematic = false;
                _rigidbody.useGravity = true;
            }

            if (_collider != null)
            {
                _collider.enabled = false;
            }
        }
        #endregion

        #region Attack
        /// <summary>
        /// Attempt to attack the player.
        /// </summary>
        /// <param name="player">Player transform</param>
        public virtual void AttackPlayer(Transform player)
        {
            if (!IsAlive || player == null) return;

            float distance = Vector3.Distance(transform.position, player.position);

            if (distance <= _attackRange && Time.time >= _lastAttackTime + _attackCooldown)
            {
                _lastAttackTime = Time.time;

                // Deal damage to player
                var playerHealth = player.GetComponent<Game.Player.PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(_meleeDamage, transform.position);
                }
            }
        }
        #endregion
    }
}
