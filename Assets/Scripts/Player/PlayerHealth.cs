using UnityEngine;
using Game.Core;

namespace Game.Player
{
    /// <summary>
    /// Manages player health, armor, and damage handling.
    /// </summary>
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        #region Health Settings
        [Header("Health Settings")]
        [SerializeField] private float _maxHealth = 100f;
        [SerializeField] private float _currentHealth;
        [SerializeField] private float _maxArmor = 100f;
        [SerializeField] private float _currentArmor = 0f;
        [SerializeField] private float _armorAbsorption = 0.5f; // 50% damage reduction

        public float MaxHealth => _maxHealth;
        public float CurrentHealth => _currentHealth;
        public float MaxArmor => _maxArmor;
        public float CurrentArmor => _currentArmor;
        public bool IsAlive => _currentHealth > 0f;
        #endregion

        #region Regeneration
        [Header("Regeneration")]
        [SerializeField] private bool _enableHealthRegen = false;
        [SerializeField] private float _healthRegenRate = 5f;
        [SerializeField] private float _healthRegenDelay = 3f;

        private float _timeSinceLastDamage;
        #endregion

        #region Events
        public static event System.Action<float, float> OnHealthChanged;
        public static event System.Action<float, float> OnArmorChanged;
        public static event System.Action OnPlayerDeath;
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            _currentHealth = _maxHealth;
            _currentArmor = 0f;
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
            OnArmorChanged?.Invoke(_currentArmor, _maxArmor);
        }

        private void Update()
        {
            if (_enableHealthRegen && _currentHealth < _maxHealth)
            {
                _timeSinceLastDamage += Time.deltaTime;

                if (_timeSinceLastDamage >= _healthRegenDelay)
                {
                    Heal(_healthRegenRate * Time.deltaTime);
                }
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get current armor value.
        /// </summary>
        public float GetCurrentArmor()
        {
            return _currentArmor;
        }

        /// <summary>
        /// Get maximum armor value.
        /// </summary>
        public float GetMaxArmor()
        {
            return _maxArmor;
        }

        /// <summary>
        /// Apply damage to the player.
        /// </summary>
        public void TakeDamage(float damage, DamageSystem.DamageType damageType, Vector3 hitPoint)
        {
            if (!IsAlive) return;

            _timeSinceLastDamage = 0f;

            // Apply armor absorption
            if (_currentArmor > 0f)
            {
                float armorDamage = damage * _armorAbsorption;
                float healthDamage = damage - armorDamage;

                _currentArmor -= armorDamage;

                if (_currentArmor < 0f)
                {
                    healthDamage += Mathf.Abs(_currentArmor);
                    _currentArmor = 0f;
                }

                _currentHealth -= healthDamage;
                OnArmorChanged?.Invoke(_currentArmor, _maxArmor);
            }
            else
            {
                _currentHealth -= damage;
            }

            _currentHealth = Mathf.Clamp(_currentHealth, 0f, _maxHealth);
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

            if (_currentHealth <= 0f)
            {
                Die();
            }
        }

        /// <summary>
        /// Heal the player.
        /// </summary>
        public void Heal(float amount)
        {
            if (!IsAlive) return;

            _currentHealth = Mathf.Min(_currentHealth + amount, _maxHealth);
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

        /// <summary>
        /// Add armor to the player.
        /// </summary>
        public void AddArmor(float amount)
        {
            _currentArmor = Mathf.Min(_currentArmor + amount, _maxArmor);
            OnArmorChanged?.Invoke(_currentArmor, _maxArmor);
        }

        /// <summary>
        /// Handle player death.
        /// </summary>
        public void Die()
        {
            if (!IsAlive) return;

            _currentHealth = 0f;
            OnPlayerDeath?.Invoke();

            // Trigger game over
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
        }
        #endregion
    }
}
