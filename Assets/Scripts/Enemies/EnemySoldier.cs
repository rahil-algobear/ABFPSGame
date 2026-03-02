using UnityEngine;
using Game.Core;
using System.Collections;

namespace Game.Enemies
{
    /// <summary>
    /// Ranged enemy that shoots at the player.
    /// </summary>
    public class EnemySoldier : EnemyBase
    {
        [Header("Soldier Settings")]
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private Transform _firePoint;
        [SerializeField] private float _projectileSpeed = 20f;
        [SerializeField] private float _fireRange = 15f;
        [SerializeField] private int _burstCount = 3;
        [SerializeField] private float _burstDelay = 0.2f;

        private bool _isFiring = false;

        protected override void Awake()
        {
            base.Awake();
            _maxHealth = 75f;
            _currentHealth = _maxHealth;
            _damage = 10f;
            _attackRange = 15f;
            _attackCooldown = 2f;
            _armor = 20f;
        }

        /// <summary>
        /// Performs a ranged attack.
        /// </summary>
        protected override void PerformAttack(Transform target)
        {
            if (_isFiring)
                return;

            StartCoroutine(FireBurst(target));
        }

        /// <summary>
        /// Fires a burst of projectiles.
        /// </summary>
        private IEnumerator FireBurst(Transform target)
        {
            _isFiring = true;

            for (int i = 0; i < _burstCount; i++)
            {
                FireProjectile(target);
                yield return new WaitForSeconds(_burstDelay);
            }

            _isFiring = false;
        }

        /// <summary>
        /// Fires a single projectile.
        /// </summary>
        private void FireProjectile(Transform target)
        {
            if (_firePoint == null || target == null)
                return;

            // Play attack sound
            if (_attackSound != null)
            {
                AudioManager.Instance.PlaySFX3D(_attackSound, transform.position);
            }

            // Calculate direction with some inaccuracy
            Vector3 direction = (target.position - _firePoint.position).normalized;
            direction += new Vector3(
                Random.Range(-0.1f, 0.1f),
                Random.Range(-0.1f, 0.1f),
                Random.Range(-0.1f, 0.1f)
            );

            // Create projectile
            if (_projectilePrefab != null)
            {
                GameObject projectile = Instantiate(
                    _projectilePrefab,
                    _firePoint.position,
                    Quaternion.LookRotation(direction)
                );

                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = direction * _projectileSpeed;
                }

                EnemyProjectile proj = projectile.GetComponent<EnemyProjectile>();
                if (proj != null)
                {
                    proj.Initialize(_damage, DamageSystem.DamageType.Bullet);
                }

                Destroy(projectile, 5f);
            }
            else
            {
                // Fallback to raycast
                if (Physics.Raycast(_firePoint.position, direction, out RaycastHit hit, _fireRange))
                {
                    Player.PlayerHealth playerHealth = hit.collider.GetComponent<Player.PlayerHealth>();
                    if (playerHealth != null)
                    {
                        playerHealth.TakeDamage(_damage, hit.point);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Enemy projectile component.
    /// </summary>
    public class EnemyProjectile : MonoBehaviour
    {
        private float _damage;
        private DamageSystem.DamageType _damageType;

        public void Initialize(float damage, DamageSystem.DamageType damageType)
        {
            _damage = damage;
            _damageType = damageType;
        }

        private void OnTriggerEnter(Collider other)
        {
            // Don't hit other enemies
            if (other.CompareTag("Enemy"))
                return;

            // Apply damage to player
            Player.PlayerHealth playerHealth = other.GetComponent<Player.PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(_damage, transform.position);
            }

            Destroy(gameObject);
        }
    }
}