using UnityEngine;
using Game.Core;

namespace Game.Enemies
{
    /// <summary>
    /// Base class for all enemy types.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class EnemyBase : MonoBehaviour, Game.Core.IDamageable
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
        [SerializeField] protected float _damage = 10f;
        [SerializeField] protected float _attackRange = 2f;
        [SerializeField] protected float _attackCooldown = 1f;
        [SerializeField] protected float _armor = 0f;

        protected float _lastAttackTime;
        #endregion

        #region Score
        [Header("Score")]
        [SerializeField] protected int _scoreValue = 100;

        public int ScoreValue => _scoreValue;
        #endregion

        #region Audio
        [Header("Audio")]
        [SerializeField] protected AudioClip _attackSound;
        [SerializeField] protected AudioClip _hurtSound;
        [SerializeField] protected AudioClip _deathSound;
        #endregion

        #region Components
        protected Rigidbody _rigidbody;
        protected Collider _collider;
        protected EnemyAI _ai;
        protected UnityEngine.AI.NavMeshAgent _agent;
        protected Animator _animator;
        #endregion

        #region Unity Lifecycle
        protected virtual void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
            _ai = GetComponent<EnemyAI>();
            _agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            _animator = GetComponent<Animator>();

            _currentHealth = _maxHealth;
        }

        protected virtual void Start()
        {
            // Override in subclasses if needed
        }
        #endregion

        #region IDamageable Implementation
        /// <summary>
        /// Take damage from a weapon hit.
        /// </summary>
        public virtual void TakeDamage(float damage, DamageSystem.DamageType damageType, Vector3 hitPoint)
        {
            if (!IsAlive) return;

            // Apply armor reduction
            float finalDamage = Mathf.Max(damage - _armor, damage * 0.1f);
            _currentHealth -= finalDamage;

            // Play hurt sound
            if (_hurtSound != null)
            {
                AudioManager.Instance.PlaySFX3D(_hurtSound, transform.position);
            }

            // Alert AI
            if (_ai != null)
            {
                Vector3 hitDirection = (transform.position - hitPoint).normalized;
                _ai.OnDamageTaken(hitPoint, hitDirection);
            }

            if (_currentHealth <= 0f)
            {
                Die();
            }
        }

        /// <summary>
        /// Handle enemy death.
        /// </summary>
        public virtual void Die()
        {
            if (!IsAlive) return;

            _currentHealth = 0f;

            // Play death sound
            if (_deathSound != null)
            {
                AudioManager.Instance.PlaySFX3D(_deathSound, transform.position);
            }

            // Register kill
            GameManager.Instance.RegisterEnemyKill(_scoreValue);

            // Disable AI
            if (_ai != null)
            {
                _ai.enabled = false;
            }

            if (_agent != null)
            {
                _agent.enabled = false;
            }

            // Enable ragdoll or play death animation
            EnableRagdoll();

            // Destroy after delay
            Destroy(gameObject, 5f);
        }
        #endregion

        #region Death
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
        public virtual void AttackPlayer(Transform player)
        {
            if (!IsAlive || player == null) return;

            float distance = Vector3.Distance(transform.position, player.position);

            if (distance <= _attackRange && Time.time >= _lastAttackTime + _attackCooldown)
            {
                _lastAttackTime = Time.time;
                PerformAttack(player);
            }
        }

        /// <summary>
        /// Perform the actual attack. Override in subclasses.
        /// </summary>
        protected virtual void PerformAttack(Transform target)
        {
            // Default melee attack
            var playerHealth = target.GetComponent<Game.Player.PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(_damage, transform.position);
            }
        }
        #endregion
    }
}