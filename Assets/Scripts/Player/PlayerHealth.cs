using UnityEngine;
using System;
using Game.Core;

namespace Game.Player
{
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        [Header("Health")]
        [SerializeField] private float _maxHealth = 100f;
        [SerializeField] private float _currentHealth;

        [Header("Armor")]
        [SerializeField] private float _maxArmor = 100f;
        [SerializeField] private float _currentArmor;

        [Header("Regeneration")]
        [SerializeField] private bool _canRegenerate = true;
        [SerializeField] private float _regenDelay = 5f;
        [SerializeField] private float _regenRate = 5f;

        private float _lastDamageTime;
        private bool _isDead;

        public static event Action<float, float> OnHealthChanged;
        public static event Action<float, float> OnArmorChanged;
        public static event Action OnPlayerDeath;

        public bool IsDead => _isDead;

        private void Start()
        {
            _currentHealth = _maxHealth;
            _currentArmor = _maxArmor;
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
            OnArmorChanged?.Invoke(_currentArmor, _maxArmor);
        }

        private void Update()
        {
            if (_canRegenerate && !_isDead && Time.time > _lastDamageTime + _regenDelay)
            {
                RegenerateHealth();
            }
        }

        public void TakeDamage(float damage, DamageSystem.DamageType damageType, Vector3 hitPoint)
        {
            if (_isDead) return;

            _lastDamageTime = Time.time;

            if (_currentArmor > 0)
            {
                float armorDamage = damage * 0.5f;
                float healthDamage = damage * 0.5f;

                _currentArmor -= armorDamage;
                if (_currentArmor < 0)
                {
                    healthDamage += Mathf.Abs(_currentArmor);
                    _currentArmor = 0;
                }

                _currentHealth -= healthDamage;
                OnArmorChanged?.Invoke(_currentArmor, _maxArmor);
            }
            else
            {
                _currentHealth -= damage;
            }

            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

            if (_currentHealth <= 0)
            {
                Die();
            }
        }

        public void Die()
        {
            if (_isDead) return;

            _isDead = true;
            _currentHealth = 0;
            OnPlayerDeath?.Invoke();
            Game.Core.GameManager.Instance.GameOver();
        }

        public void Heal(float amount)
        {
            if (_isDead) return;

            _currentHealth = Mathf.Min(_currentHealth + amount, _maxHealth);
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

        public void AddArmor(float amount)
        {
            if (_isDead) return;

            _currentArmor = Mathf.Min(_currentArmor + amount, _maxArmor);
            OnArmorChanged?.Invoke(_currentArmor, _maxArmor);
        }

        private void RegenerateHealth()
        {
            if (_currentHealth < _maxHealth)
            {
                _currentHealth = Mathf.Min(_currentHealth + _regenRate * Time.deltaTime, _maxHealth);
                OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
            }
        }

        public float GetCurrentHealth() => _currentHealth;
        public float GetMaxHealth() => _maxHealth;
    }
}