using UnityEngine;
using System;
using Game.Core;

namespace Game.Player
{
    /// <summary>
    /// Manages player health, armor, and damage handling.
    /// </summary>
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        #region Events
        public static event Action<float, float> OnHealthChanged;
        public static event Action<float, float> OnArmorChanged;
        public static event Action<float, DamageSystem.DamageType, Vector3> OnDamageTaken;
        public static event Action OnPlayerDeath;
        #endregion

        #region Health Settings
        [Header("Health Settings")]
        [SerializeField] private float _maxHealth = 100f;
        [SerializeField] private float _currentHealth;
        [SerializeField] private float _maxArmor = 100f;
        [SerializeField] private float _currentArmor;
        [SerializeField] private float _armorAbsorption = 0.66f;
        #endregion

        #region Regeneration
        [Header("Regeneration")]
        [SerializeField] private bool _enableHealthRegen = false;
        [SerializeField] private float _healthRegenRate = 5f;
        [SerializeField] private float _healthRegenDelay = 5f;
        private float _timeSinceLastDamage;
        #endregion

        #region State
        private bool _isAlive = true;
        public bool IsAlive => _isAlive;
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            _currentHealth = _maxHealth;
            _currentArmor = _maxArmor;
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
            OnArmorChanged?.Invoke(_currentArmor, _maxArmor);
        }

        private void Update()
        {
            if (!_isAlive) return;

            // Health regeneration
            if (_enableHealthRegen && _currentHealth < _maxHealth)
            {
                _timeSinceLastDamage += Time.deltaTime;
                if (_timeSinceLastDamage >= _healthRegenDelay)
                {
                    _currentHealth = Mathf.Min(_currentHealth + _healthRegenRate * Time.deltaTime, _maxHealth);
                    OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
                }
            }
        }
        #endregion

        #region IDamageable Implementation
        public void TakeDamage(float damage, DamageSystem.DamageType damageType, Vector3 hitPoint)
        {
            if (!_isAlive || damage <= 0f) return;

            _timeSinceLastDamage = 0f;

            // Apply armor absorption
            if (_currentArmor > 0f)
            {
                float armorDamage = damage * _armorAbsorption;
                float healthDamage = damage - armorDamage;

                _currentArmor = Mathf.Max(0f, _currentArmor - armorDamage);
                _currentHealth = Mathf.Max(0f, _currentHealth - healthDamage);

                OnArmorChanged?.Invoke(_currentArmor, _maxArmor);
            }
            else
            {
                _currentHealth = Mathf.Max(0f, _currentHealth - damage);
            }

            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
            OnDamageTaken?.Invoke(damage, damageType, hitPoint);

            if (_currentHealth <= 0f)
            {
                Die();
            }
        }

        public void Die()
        {
            if (!_isAlive) return;

            _isAlive = false;
            OnPlayerDeath?.Invoke();

            // Notify GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
        }
        #endregion

        #region Public Methods
        public void Heal(float amount)
        {
            if (!_isAlive || amount <= 0f) return;

            _currentHealth = Mathf.Min(_currentHealth + amount, _maxHealth);
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

        public void AddArmor(float amount)
        {
            if (!_isAlive || amount <= 0f) return;

            _currentArmor = Mathf.Min(_currentArmor + amount, _maxArmor);
            OnArmorChanged?.Invoke(_currentArmor, _maxArmor);
        }

        public float GetCurrentHealth()
        {
            return _currentHealth;
        }

        public float GetMaxHealth()
        {
            return _maxHealth;
        }

        public float GetCurrentArmor()
        {
            return _currentArmor;
        }

        public float GetMaxArmor()
        {
            return _maxArmor;
        }
        #endregion
    }
}