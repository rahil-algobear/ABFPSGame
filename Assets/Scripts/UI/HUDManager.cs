using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.Core;
using Game.Player;

namespace Game.UI
{
    /// <summary>
    /// Manages HUD display elements like health, ammo, and score.
    /// </summary>
    public class HUDManager : MonoBehaviour
    {
        #region UI Elements
        [Header("Health")]
        [SerializeField] private Image _healthBar;
        [SerializeField] private TextMeshProUGUI _healthText;

        [Header("Armor")]
        [SerializeField] private Image _armorBar;
        [SerializeField] private TextMeshProUGUI _armorText;

        [Header("Ammo")]
        [SerializeField] private TextMeshProUGUI _ammoText;

        [Header("Score")]
        [SerializeField] private TextMeshProUGUI _scoreText;

        [Header("Crosshair")]
        [SerializeField] private Image _crosshair;
        #endregion

        #region References
        private PlayerHealth _playerHealth;
        private Game.Player.WeaponManager _weaponManager;
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            InitializeReferences();
            SubscribeToEvents();
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        private void Update()
        {
            UpdateAmmoDisplay();
        }
        #endregion

        #region Initialization
        private void InitializeReferences()
        {
            _playerHealth = FindObjectOfType<PlayerHealth>();
            _weaponManager = FindObjectOfType<Game.Player.WeaponManager>();
        }

        private void SubscribeToEvents()
        {
            PlayerHealth.OnHealthChanged += UpdateHealthDisplay;
            PlayerHealth.OnArmorChanged += UpdateArmorDisplay;

            if (GameManager.Instance != null)
            {
                GameManager.OnScoreChanged += UpdateScoreDisplay;
            }
        }

        private void UnsubscribeFromEvents()
        {
            PlayerHealth.OnHealthChanged -= UpdateHealthDisplay;
            PlayerHealth.OnArmorChanged -= UpdateArmorDisplay;

            if (GameManager.Instance != null)
            {
                GameManager.OnScoreChanged -= UpdateScoreDisplay;
            }
        }
        #endregion

        #region Display Updates
        private void UpdateHealthDisplay(float current, float max)
        {
            if (_healthBar != null)
            {
                _healthBar.fillAmount = current / max;
            }

            if (_healthText != null)
            {
                _healthText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
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
                _armorText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
            }
        }

        private void UpdateAmmoDisplay()
        {
            if (_weaponManager == null || _ammoText == null) return;

            var currentWeapon = _weaponManager.GetCurrentWeapon();
            if (currentWeapon != null)
            {
                _ammoText.text = $"{currentWeapon.GetCurrentAmmo()} / {currentWeapon.GetReserveAmmo()}";
            }
        }

        private void UpdateScoreDisplay(int score)
        {
            if (_scoreText != null)
            {
                _scoreText.text = $"Score: {score}";
            }
        }
        #endregion

        #region Public Methods
        public void ShowCrosshair(bool show)
        {
            if (_crosshair != null)
            {
                _crosshair.enabled = show;
            }
        }
        #endregion
    }
}