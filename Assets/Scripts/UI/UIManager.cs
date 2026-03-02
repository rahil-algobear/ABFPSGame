using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.Core;
using Game.Player;

namespace Game.UI
{
    /// <summary>
    /// Main UI manager coordinating all UI elements.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        #region Singleton
        private static UIManager _instance;
        public static UIManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<UIManager>();
                }
                return _instance;
            }
        }
        #endregion

        #region UI References
        [Header("UI Panels")]
        [SerializeField] private GameObject _hudPanel;
        [SerializeField] private GameObject _pauseMenuPanel;
        [SerializeField] private GameObject _gameOverPanel;
        [SerializeField] private GameObject _levelCompletePanel;

        [Header("HUD Elements")]
        [SerializeField] private TextMeshProUGUI _healthText;
        [SerializeField] private TextMeshProUGUI _armorText;
        [SerializeField] private TextMeshProUGUI _ammoText;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private Image _healthBar;
        [SerializeField] private Image _armorBar;
        [SerializeField] private Image _crosshair;

        [Header("Game Over")]
        [SerializeField] private TextMeshProUGUI _finalScoreText;
        [SerializeField] private TextMeshProUGUI _enemiesKilledText;
        #endregion

        #region References
        private PlayerHealth _playerHealth;
        private Game.Player.WeaponManager _weaponManager;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
        }

        private void Start()
        {
            InitializeUI();
            SubscribeToEvents();
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }
        #endregion

        #region Initialization
        private void InitializeUI()
        {
            _playerHealth = FindObjectOfType<PlayerHealth>();
            _weaponManager = FindObjectOfType<Game.Player.WeaponManager>();

            ShowHUD();
            HidePauseMenu();
            HideGameOver();
            HideLevelComplete();
        }

        private void SubscribeToEvents()
        {
            PlayerHealth.OnHealthChanged += UpdateHealthUI;
            PlayerHealth.OnArmorChanged += UpdateArmorUI;
            PlayerHealth.OnDamageTaken += OnPlayerDamaged;
            PlayerHealth.OnPlayerDeath += ShowGameOver;

            GameManager.OnScoreChanged += UpdateScore;
            GameManager.OnGamePaused += ShowPauseMenu;
            GameManager.OnGameResumed += HidePauseMenu;
            GameManager.OnLevelComplete += ShowLevelComplete;
            GameManager.OnGameOver += ShowGameOver;
        }

        private void UnsubscribeFromEvents()
        {
            PlayerHealth.OnHealthChanged -= UpdateHealthUI;
            PlayerHealth.OnArmorChanged -= UpdateArmorUI;
            PlayerHealth.OnDamageTaken -= OnPlayerDamaged;
            PlayerHealth.OnPlayerDeath -= ShowGameOver;

            GameManager.OnScoreChanged -= UpdateScore;
            GameManager.OnGamePaused -= ShowPauseMenu;
            GameManager.OnGameResumed -= HidePauseMenu;
            GameManager.OnLevelComplete -= ShowLevelComplete;
            GameManager.OnGameOver -= ShowGameOver;
        }
        #endregion

        #region HUD Updates
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

        private void UpdateAmmoUI(int current, int reserve)
        {
            if (_ammoText != null)
            {
                _ammoText.text = $"{current} / {reserve}";
            }
        }

        private void UpdateScore(int score)
        {
            if (_scoreText != null)
            {
                _scoreText.text = $"Score: {score}";
            }
        }

        private void OnPlayerDamaged(float damage, DamageSystem.DamageType damageType, Vector3 hitPoint)
        {
            // Flash red or show damage indicator
        }

        private void Update()
        {
            if (_weaponManager != null)
            {
                var currentWeapon = _weaponManager.GetCurrentWeapon();
                if (currentWeapon != null)
                {
                    UpdateAmmoUI(currentWeapon.GetCurrentAmmo(), currentWeapon.GetReserveAmmo());
                }
            }
        }
        #endregion

        #region Panel Management
        private void ShowHUD()
        {
            if (_hudPanel != null)
            {
                _hudPanel.SetActive(true);
            }
        }

        private void HideHUD()
        {
            if (_hudPanel != null)
            {
                _hudPanel.SetActive(false);
            }
        }

        private void ShowPauseMenu()
        {
            if (_pauseMenuPanel != null)
            {
                _pauseMenuPanel.SetActive(true);
            }
        }

        private void HidePauseMenu()
        {
            if (_pauseMenuPanel != null)
            {
                _pauseMenuPanel.SetActive(false);
            }
        }

        private void ShowGameOver()
        {
            HideHUD();

            if (_gameOverPanel != null)
            {
                _gameOverPanel.SetActive(true);

                if (_finalScoreText != null && GameManager.Instance != null)
                {
                    _finalScoreText.text = $"Final Score: {GameManager.Instance.CurrentScore}";
                }

                if (_enemiesKilledText != null && GameManager.Instance != null)
                {
                    _enemiesKilledText.text = $"Enemies Killed: {GameManager.Instance.EnemiesKilled}";
                }
            }
        }

        private void HideGameOver()
        {
            if (_gameOverPanel != null)
            {
                _gameOverPanel.SetActive(false);
            }
        }

        private void ShowLevelComplete()
        {
            HideHUD();

            if (_levelCompletePanel != null)
            {
                _levelCompletePanel.SetActive(true);
            }
        }

        private void HideLevelComplete()
        {
            if (_levelCompletePanel != null)
            {
                _levelCompletePanel.SetActive(false);
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