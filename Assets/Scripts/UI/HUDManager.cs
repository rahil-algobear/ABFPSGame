using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.Player;
using Game.Core;

namespace Game.UI
{
    public class HUDManager : MonoBehaviour
    {
        [Header("Health & Armor")]
        [SerializeField] private Image _healthBar;
        [SerializeField] private Image _armorBar;
        [SerializeField] private TextMeshProUGUI _healthText;
        [SerializeField] private TextMeshProUGUI _armorText;

        [Header("Ammo")]
        [SerializeField] private TextMeshProUGUI _ammoText;

        [Header("Score")]
        [SerializeField] private TextMeshProUGUI _scoreText;

        [Header("Crosshair")]
        [SerializeField] private Image _crosshair;

        private PlayerHealth _playerHealth;
        private Game.Player.WeaponManager _weaponManager;

        private void Start()
        {
            _playerHealth = FindObjectOfType<PlayerHealth>();
            _weaponManager = FindObjectOfType<Game.Player.WeaponManager>();

            if (_playerHealth != null)
            {
                PlayerHealth.OnHealthChanged += UpdateHealthDisplay;
                PlayerHealth.OnArmorChanged += UpdateArmorDisplay;
            }

            if (_weaponManager != null)
            {
                UpdateAmmoDisplay();
            }

            GameManager.OnScoreChanged += UpdateScoreDisplay;
        }

        private void OnDestroy()
        {
            if (_playerHealth != null)
            {
                PlayerHealth.OnHealthChanged -= UpdateHealthDisplay;
                PlayerHealth.OnArmorChanged -= UpdateArmorDisplay;
            }

            GameManager.OnScoreChanged -= UpdateScoreDisplay;
        }

        private void Update()
        {
            UpdateAmmoDisplay();
        }

        private void UpdateHealthDisplay(float current, float max)
        {
            if (_healthBar != null)
            {
                _healthBar.fillAmount = current / max;
            }

            if (_healthText != null)
            {
                _healthText.text = $"{Mathf.CeilToInt(current)}";
            }
        }

        private void UpdateArmorDisplay(float current, float max)
        {
            if (_armorBar != null)
            {
                _armorBar.fillAmount = current / max;
            }

            if (_armorText != null)
            {
                _armorText.text = $"{Mathf.CeilToInt(current)}";
            }
        }

        private void UpdateAmmoDisplay()
        {
            if (_weaponManager == null || _ammoText == null) return;

            var currentWeapon = _weaponManager.GetCurrentWeapon();
            if (currentWeapon != null)
            {
                int currentAmmo = currentWeapon.GetCurrentAmmo();
                int reserveAmmo = currentWeapon.GetReserveAmmo();
                _ammoText.text = $"{currentAmmo} / {reserveAmmo}";
            }
        }

        private void UpdateScoreDisplay(int score)
        {
            if (_scoreText != null)
            {
                _scoreText.text = $"Score: {score}";
            }
        }
    }
}