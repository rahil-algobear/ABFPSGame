using UnityEngine;
using Game.Core;

namespace Game.Enemies
{
    public class EnemyGrunt : EnemyBase
    {
        [Header("Grunt Specific")]
        [SerializeField] private float _chargeSpeed = 6f;
        [SerializeField] private float _chargeRange = 5f;

        private EnemyAI _ai;
        private bool _isCharging;

        protected override void Awake()
        {
            base.Awake();
            _ai = GetComponent<EnemyAI>();
        }

        private void Update()
        {
            if (!_isAlive || _player == null) return;

            float distanceToPlayer = Vector3.Distance(transform.position, _player.position);

            if (distanceToPlayer <= _chargeRange && !_isCharging)
            {
                StartCharge();
            }

            if (_isCharging)
            {
                ChargeAtPlayer();
            }
        }

        private void StartCharge()
        {
            _isCharging = true;
        }

        private void ChargeAtPlayer()
        {
            Vector3 direction = (_player.position - transform.position).normalized;
            transform.position += direction * _chargeSpeed * Time.deltaTime;

            float distanceToPlayer = Vector3.Distance(transform.position, _player.position);
            if (distanceToPlayer <= _attackRange)
            {
                Attack();
                _isCharging = false;
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