using UnityEngine;
using Game.Core;

namespace Game.Enemies
{
    /// <summary>
    /// Fast melee enemy that rushes the player.
    /// </summary>
    public class EnemyGrunt : EnemyBase
    {
        [Header("Grunt Settings")]
        [SerializeField] private float _rushSpeed = 8f;
        [SerializeField] private float _normalSpeed = 3.5f;

        protected override void Awake()
        {
            base.Awake();
            _maxHealth = 50f;
            _currentHealth = _maxHealth;
            _damage = 15f;
            _attackRange = 2f;
            _attackCooldown = 1.5f;
        }

        protected override void Start()
        {
            base.Start();
            if (_agent != null)
            {
                _agent.speed = _normalSpeed;
            }
        }

        /// <summary>
        /// Performs a melee attack.
        /// </summary>
        protected override void PerformAttack(Transform target)
        {
            // Play attack animation
            if (_animator != null)
            {
                _animator.SetTrigger("Attack");
            }

            // Play attack sound
            if (_attackSound != null)
            {
                AudioManager.Instance.PlaySFX3D(_attackSound, transform.position);
            }

            // Apply damage to player
            Player.PlayerHealth playerHealth = target.GetComponent<Player.PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(_damage, DamageSystem.DamageType.Melee, transform.position);
            }
        }

        /// <summary>
        /// Increases speed when chasing.
        /// </summary>
        private void Update()
        {
            if (_ai != null && _agent != null)
            {
                if (_ai.GetCurrentState() == EnemyAI.AIState.Chase)
                {
                    _agent.speed = _rushSpeed;
                }
                else
                {
                    _agent.speed = _normalSpeed;
                }
            }
        }
    }
}