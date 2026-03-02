using UnityEngine;
using System;
using Game.Core;

namespace Game.Player
{
    /// <summary>
    /// Manages player health, damage, and regeneration.
    /// </summary>
    public class PlayerHealth : MonoBehaviour
    {
        #region Events
        public static event Action<float, float> OnHealthChanged;
        public static event Action<Vector3> OnDamageTaken;
        public static event Action OnPlayerDeath;
        #endregion

        #region Health Settings
        [Header("Health Settings")]
        [SerializeField] private float _maxHealth = 100f;
        [SerializeField] private float _currentHealth = 100f;

        public float MaxHealth => _maxHealth;
        public float CurrentHealth => _currentHealth;
        public float HealthPercentage => _currentHealth / _maxHealth;
        #endregion

        #region Regeneration
        [Header("Regeneration")]
        [SerializeField] private bool _enableRegeneration = true;
        [SerializeField] private float _regenDelay = 3f;
        [SerializeField] private float _regenRate = 5f;
        [SerializeField] private float _regenAmount = 1f;

        private float _timeSinceLastDamage;
        #endregion

        #region Damage Flash
        [Header("Damage Flash")]
        [SerializeField] private bool _enableDamageFlash = true;
        [SerializeField] private float _flashDuration = 0.2f;
        [SerializeField] private Color _flashColor = new Color(1f, 0f, 0f, 0.3f);

        private Material _flashMaterial;
        private float _flashTimer;
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            _currentHealth = _maxHealth;
            _timeSinceLastDamage = 0f;

            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

        private void Update()
        {
            if (GameManager.Instance.IsPaused || GameManager.Instance.IsGameOver)
            {
                return;
            }

            HandleRegeneration();
            HandleDamageFlash();
        }
        #endregion

        #region Regeneration
        /// <summary>
        /// Handle health regeneration over time.
        /// </summary>
        private void HandleRegeneration()
        {
            if (!_enableRegeneration || _currentHealth >= _maxHealth)
            {
                return;
            }

            _timeSinceLastDamage += Time.deltaTime;

            if (_timeSinceLastDamage >= _regenDelay)
            {
                _currentHealth += _regenAmount * _regenRate * Time.deltaTime;
                _currentHealth = Mathf.Min(_currentHealth, _maxHealth);
                OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
            }
        }
        #endregion

        #region Damage Flash
        /// <summary>
        /// Handle damage flash effect.
        /// </summary>
        private void HandleDamageFlash()
        {
            if (_flashTimer > 0f)
            {
                _flashTimer -= Time.deltaTime;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Apply damage to the player.
        /// </summary>
        /// <param name="damage">Damage amount</param>
        /// <param name="damageSource">Position of damage source for directional indicator</param>
        public void TakeDamage(float damage, Vector3 damageSource = default)
        {
            if (_currentHealth <= 0f) return;

            _currentHealth -= damage;
            _currentHealth = Mathf.Max(_currentHealth, 0f);
            _timeSinceLastDamage = 0f;

            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
            OnDamageTaken?.Invoke(damageSource);

            // Trigger damage flash
            if (_enableDamageFlash)
            {
                _flashTimer = _flashDuration;
            }

            // Check for death
            if (_currentHealth <= 0f)
            {
                Die();
            }
        }

        /// <summary>
        /// Heal the player.
        /// </summary>
        /// <param name="amount">Heal amount</param>
        public void Heal(float amount)
        {
            if (_currentHealth >= _maxHealth) return;

            _currentHealth += amount;
            _currentHealth = Mathf.Min(_currentHealth, _maxHealth);

            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

        /// <summary>
        /// Set health to a specific value.
        /// </summary>
        /// <param name="health">New health value</param>
        public void SetHealth(float health)
        {
            _currentHealth = Mathf.Clamp(health, 0f, _maxHealth);
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

            if (_currentHealth <= 0f)
            {
                Die();
            }
        }

        /// <summary>
        /// Check if player is alive.
        /// </summary>
        /// <returns>True if alive</returns>
        public bool IsAlive()
        {
            return _currentHealth > 0f;
        }

        /// <summary>
        /// Get damage flash alpha for UI.
        /// </summary>
        /// <returns>Flash alpha value</returns>
        public float GetDamageFlashAlpha()
        {
            if (_flashTimer > 0f)
            {
                return Mathf.Lerp(0f, _flashColor.a, _flashTimer / _flashDuration);
            }
            return 0f;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Handle player death.
        /// </summary>
        private void Die()
        {
            OnPlayerDeath?.Invoke();
            GameManager.Instance.GameOver();
        }
        #endregion
    }
}
