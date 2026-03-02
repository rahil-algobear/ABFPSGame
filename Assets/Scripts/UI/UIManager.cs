using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.Core;

namespace Game.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("HUD")]
        [SerializeField] private TextMeshProUGUI _healthText;
        [SerializeField] private TextMeshProUGUI _armorText;
        [SerializeField] private TextMeshProUGUI _ammoText;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private Image _healthBar;
        [SerializeField] private Image _armorBar;
        [SerializeField] private Image _crosshair;

        [Header("Menus")]
        [SerializeField] private GameObject _pauseMenu;
        [SerializeField] private GameObject _gameOverMenu;
        [SerializeField] private GameObject _levelCompleteMenu;

        [Header("Damage Indicator")]
        [SerializeField] private Image _damageOverlay;
        [SerializeField] private float _damageFadeDuration = 0.5f;

        private Game.Player.WeaponManager _weaponManager;
        private Game.Player.PlayerHealth _playerHealth;
        private float _damageAlpha;

        private void Start()
        {
            _weaponManager = FindObjectOfType<Game.Player.WeaponManager>();
            _playerHealth = FindObjectOfType<Game.Player.PlayerHealth>();

            if (_pauseMenu != null) _pauseMenu.SetActive(false);
            if (_gameOverMenu != null) _gameOverMenu.SetActive(false);
            if (_levelCompleteMenu != null) _levelCompleteMenu.SetActive(false);

            GameManager.OnGamePaused += ShowPauseMenu;
            GameManager.OnGameResumed += HidePauseMenu;
            GameManager.OnGameOver += ShowGameOverMenu;
            GameManager.OnLevelComplete += ShowLevelCompleteMenu;

            if (_playerHealth != null)
            {
                Game.Player.PlayerHealth.OnHealthChanged += UpdateHealthUI;
                Game.Player.PlayerHealth.OnArmorChanged += UpdateArmorUI;
            }

            if (_weaponManager != null)
            {
                UpdateAmmoUI();
            }
        }

        private void OnDestroy()
        {
            GameManager.OnGamePaused -= ShowPauseMenu;
            GameManager.OnGameResumed -= HidePauseMenu;
            GameManager.OnGameOver -= ShowGameOverMenu;
            GameManager.OnLevelComplete -= ShowLevelCompleteMenu;

            if (_playerHealth != null)
            {
                Game.Player.PlayerHealth.OnHealthChanged -= UpdateHealthUI;
                Game.Player.PlayerHealth.OnArmorChanged -= UpdateArmorUI;
            }
        }

        private void Update()
        {
            if (_damageAlpha > 0)
            {
                _damageAlpha -= Time.deltaTime / _damageFadeDuration;
                if (_damageOverlay != null)
                {
                    Color color = _damageOverlay.color;
                    color.a = _damageAlpha;
                    _damageOverlay.color = color;
                }
            }

            UpdateAmmoUI();
        }

        private void UpdateHealthUI(float current, float max)
        {
            if (_healthText != null)
            {
                _healthText.text = $"Health: {Mathf.CeilToInt(current)}";
            }

            if (_healthBar != null)
            {
                _healthBar.fillAmount = current / max;
            }
        }

        private void UpdateArmorUI(float current, float max)
        {
            if (_armorText != null)
            {
                _armorText.text = $"Armor: {Mathf.CeilToInt(current)}";
            }

            if (_armorBar != null)
            {
                _armorBar.fillAmount = current / max;
            }
        }

        private void UpdateAmmoUI()
        {
            if (_weaponManager == null || _ammoText == null) return;

            var currentWeapon = _weaponManager.GetCurrentWeapon();
            if (currentWeapon != null)
            {
                _ammoText.text = $"{currentWeapon.GetCurrentAmmo()} / {currentWeapon.GetReserveAmmo()}";
            }
        }

        public void ShowDamageIndicator()
        {
            _damageAlpha = 1f;
        }

        private void ShowPauseMenu()
        {
            if (_pauseMenu != null)
            {
                _pauseMenu.SetActive(true);
            }
        }

        private void HidePauseMenu()
        {
            if (_pauseMenu != null)
            {
                _pauseMenu.SetActive(false);
            }
        }

        private void ShowGameOverMenu()
        {
            if (_gameOverMenu != null)
            {
                _gameOverMenu.SetActive(true);
            }
        }

        private void ShowLevelCompleteMenu()
        {
            if (_levelCompleteMenu != null)
            {
                _levelCompleteMenu.SetActive(true);
            }
        }

        public void OnResumeButtonClicked()
        {
            GameManager.Instance.ResumeGame();
        }

        public void OnRestartButtonClicked()
        {
            GameManager.Instance.RestartLevel();
        }

        public void OnQuitButtonClicked()
        {
            GameManager.Instance.QuitGame();
        }
    }
}