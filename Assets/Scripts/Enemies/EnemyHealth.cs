using UnityEngine;
using Game.Core;

namespace Game.Enemies
{
    /// <summary>
    /// Enemy health management with damage handling and death.
    /// </summary>
    public class EnemyHealth : MonoBehaviour
    {
        #region Health
        [Header("Health")]
        [SerializeField] private float _maxHealth = 100f;
        [SerializeField] private float _currentHealth;
        #endregion

        #region Death
        [Header("Death")]
        [SerializeField] private GameObject _deathEffectPrefab;
        [SerializeField] private float _deathEffectDuration = 2f;
        #endregion

        #region State
        private bool _isDead = false;
        private EnemyBase _enemyBase;
        #endregion

        #region Properties
        public float CurrentHealth => _currentHealth;
        public float MaxHealth => _maxHealth;
        public bool IsDead => _isDead;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            _enemyBase = GetComponent<EnemyBase>();
            _currentHealth = _maxHealth;
        }
        #endregion

        #region Damage
        /// <summary>
        /// Apply damage to enemy.
        /// </summary>
        /// <param name="damage">Damage amount</param>
        /// <param name="hitPoint">Hit position</param>
        /// <param name="hitDirection">Hit direction</param>
        public void TakeDamage(float damage, Vector3 hitPoint, Vector3 hitDirection)
        {
            if (_isDead) return;

            _currentHealth -= damage;

            // Show damage number
            ShowDamageNumber(damage, hitPoint);

            if (_currentHealth <= 0)
            {
                Die();
            }
        }

        /// <summary>
        /// Show floating damage number.
        /// </summary>
        private void ShowDamageNumber(float damage, Vector3 position)
        {
            // TODO: Implement floating damage numbers
            Debug.Log($"Enemy took {damage:F0} damage");
        }
        #endregion

        #region Death
        /// <summary>
        /// Handle enemy death.
        /// </summary>
        private void Die()
        {
            if (_isDead) return;

            _isDead = true;

            // Register kill with game manager
            GameManager.Instance.RegisterEnemyKill();

            // Add score
            if (_enemyBase != null)
            {
                GameManager.Instance.AddScore(_enemyBase.ScoreValue);
            }

            // Spawn death effect
            if (_deathEffectPrefab != null)
            {
                GameObject effect = Instantiate(_deathEffectPrefab, transform.position, Quaternion.identity);
                Destroy(effect, _deathEffectDuration);
            }

            // Destroy enemy
            Destroy(gameObject);
        }
        #endregion
    }
}
