using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.Core;
using Game.Player;

namespace Game.UI
{
    /// <summary>
    /// Manages the main HUD display.
    /// </summary>
    public class HUDManager : MonoBehaviour
    {
        #region UI References
        [Header("Health UI")]
        [SerializeField] private Image _healthBar;
        [SerializeField] private TextMeshProUGUI _healthText;

        [Header("Armor UI")]
        [SerializeField] private Image _armorBar;
        [SerializeField] private TextMeshProUGUI _armorText;

        [Header("Ammo UI")]
        [SerializeField] private TextMeshProUGUI _ammoText;
        [SerializeField] private TextMeshProUGUI _weaponNameText;

        [Header("Crosshair")]
        [SerializeField] private Image _crosshair;
        [SerializeField] private float _crosshairSpreadMultiplier = 10f;

        [Header("Score UI")]
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _killCountText;

        [Header("Minimap")]
        [SerializeField] private RawImage _minimapImage;
        #endregion

        #region Player References
        private PlayerHealth _playerHealth;
        private WeaponManager _weaponManager;
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            // Find player components
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _playerHealth = player.GetComponent<PlayerHealth>();
                _weaponManager = player.GetComponentInChildren<WeaponManager>();

                // Subscribe to events
                if (_playerHealth != null)
                {
                    _playerHealth.OnHealthChanged += UpdateHealthDisplay;
                    _playerHealth.OnArmorChanged += UpdateArmorDisplay;
                }

                if (_weaponManager != null)
                {
                    _weaponManager.OnAmmoChanged += UpdateAmmoDisplay;
                    _weaponManager.OnWeaponChanged += UpdateWeaponDisplay;
                }
            }

            // Subscribe to game manager events
            GameManager.OnScoreChanged += UpdateScoreDisplay;
            GameManager.OnEnemyKilled += UpdateKillCount;

            // Initial update
            UpdateAllDisplays();
        }

        private void OnDestroy()
        {
            // Unsubscribe from events
            if (_playerHealth != null)
            {
                _playerHealth.OnHealthChanged -= UpdateHealthDisplay;
                _playerHealth.OnArmorChanged -= UpdateArmorDisplay;
            }

            if (_weaponManager != null)
            {
                _weaponManager.OnAmmoChanged -= UpdateAmmoDisplay;
                _weaponManager.OnWeaponChanged -= UpdateWeaponDisplay;
            }

            GameManager.OnScoreChanged -= UpdateScoreDisplay;
            GameManager.OnEnemyKilled -= UpdateKillCount;
        }
        #endregion

        #region Display Updates
        /// <summary>
        /// Updates all HUD displays.
        /// </summary>
        private void UpdateAllDisplays()
        {
            if (_playerHealth != null)
            {
                UpdateHealthDisplay(_playerHealth.GetCurrentHealth(), _playerHealth.GetMaxHealth());
                UpdateArmorDisplay(_playerHealth.GetCurrentArmor(), _playerHealth.GetMaxArmor());
            }

            if (_weaponManager != null && _weaponManager.GetCurrentWeapon() != null)
            {
                var weapon = _weaponManager.GetCurrentWeapon();
                UpdateAmmoDisplay(weapon.GetCurrentAmmo(), weapon.GetReserveAmmo());
                UpdateWeaponDisplay(weapon);
            }

            UpdateScoreDisplay(GameManager.Instance.Score);
            UpdateKillCount(GameManager.Instance.EnemiesKilled);
        }

        /// <summary>
        /// Updates the health bar display.
        /// </summary>
        private void UpdateHealthDisplay(float currentHealth, float maxHealth)
        {
            if (_healthBar != null)
            {
                _healthBar.fillAmount = currentHealth / maxHealth;
            }

            if (_healthText != null)
            {
                _healthText.text = $"{Mathf.CeilToInt(currentHealth)}";
            }
        }

        /// <summary>
        /// Updates the armor bar display.
        /// </summary>
        private void UpdateArmorDisplay(float currentArmor, float maxArmor)
        {
            if (_armorBar != null)
            {
                _armorBar.fillAmount = currentArmor / maxArmor;
            }

            if (_armorText != null)
            {
                _armorText.text = $"{Mathf.CeilToInt(currentArmor)}";
            }
        }

        /// <summary>
        /// Updates the ammo display.
        /// </summary>
        private void UpdateAmmoDisplay(int currentAmmo, int reserveAmmo)
        {
            if (_ammoText != null)
            {
                _ammoText.text = $"{currentAmmo} / {reserveAmmo}";
            }
        }

        /// <summary>
        /// Updates the weapon name display.
        /// </summary>
        private void UpdateWeaponDisplay(Weapons.WeaponBase weapon)
        {
            if (_weaponNameText != null && weapon != null)
            {
                _weaponNameText.text = weapon.name;
            }
        }

        /// <summary>
        /// Updates the score display.
        /// </summary>
        private void UpdateScoreDisplay(int score)
        {
            if (_scoreText != null)
            {
                _scoreText.text = $"Score: {score}";
            }
        }

        /// <summary>
        /// Updates the kill count display.
        /// </summary>
        private void UpdateKillCount(int kills)
        {
            if (_killCountText != null)
            {
                _killCountText.text = $"Kills: {kills}";
            }
        }
        #endregion
    }
}