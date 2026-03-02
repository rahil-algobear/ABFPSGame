using UnityEngine;
using Game.Core;

namespace Game.Enemies
{
    public class EnemySoldier : EnemyBase
    {
        [Header("Soldier Specific")]
        [SerializeField] private float _shootRange = 20f;
        [SerializeField] private float _shootCooldown = 1.5f;
        [SerializeField] private float _bulletDamage = 15f;
        [SerializeField] private Transform _firePoint;
        [SerializeField] private GameObject _bulletPrefab;

        private float _lastShootTime;

        private void Update()
        {
            if (!_isAlive || _player == null) return;

            float distanceToPlayer = Vector3.Distance(transform.position, _player.position);

            if (distanceToPlayer <= _shootRange)
            {
                FacePlayer();
                TryShoot();
            }
        }

        private void FacePlayer()
        {
            Vector3 direction = (_player.position - transform.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            }
        }

        private void TryShoot()
        {
            if (Time.time < _lastShootTime + _shootCooldown)
                return;

            _lastShootTime = Time.time;
            Shoot();
        }

        private void Shoot()
        {
            if (_firePoint == null || _player == null) return;

            Vector3 direction = (_player.position - _firePoint.position).normalized;
            Ray ray = new Ray(_firePoint.position, direction);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, _shootRange))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    var playerHealth = hit.collider.GetComponent<Game.Player.PlayerHealth>();
                    if (playerHealth != null)
                    {
                        playerHealth.TakeDamage(_bulletDamage, DamageSystem.DamageType.Bullet, hit.point);
                    }
                }
            }
        }

        protected override void Attack()
        {
            base.Attack();
            var playerHealth = _player.GetComponent<Game.Player.PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(_attackDamage, DamageSystem.DamageType.Melee, transform.position);
            }
        }
    }
}