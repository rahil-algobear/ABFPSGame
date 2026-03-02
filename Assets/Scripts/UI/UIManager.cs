using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.Core;
using Game.Player;
using Game.Weapons;

namespace Game.UI
{
    /// <summary>
    /// Manages all UI elements and updates.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        #region HUD Elements
        [Header("HUD")]
        [SerializeField] private GameObject _hudPanel;
        [SerializeField] private Image _crosshair;
        [SerializeField] private Image _healthBar;
        [SerializeField] private TextMeshProUGUI _healthText;
        [SerializeField] private TextMeshProUGUI _ammoText;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private Image _damageVignette;
        #endregion

        #region Pause Menu
        [Header("Pause Menu")]
        [SerializeField] private GameObject _pauseMenuPanel;
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _quitButton;
        #endregion

        #region Game Over
        [Header("Game Over")]
        [SerializeField] private GameObject _gameOverPanel;
        [SerializeField] private TextMeshProUGUI _finalScoreText;
        [SerializeField] private TextMeshProUGUI _killsText;
        [SerializeField] private TextMeshProUGUI _timeText;
        [SerializeField] private Button _gameOverRestartButton;
        [SerializeField] private Button _gameOverQuitButton;
        #endregion

        #region Level Complete
        [Header("Level Complete")]
        [SerializeField] private GameObject _levelCompletePanel;
        [SerializeField] private TextMeshProUGUI _completeScoreText;
        [SerializeField] private TextMeshProUGUI _completeKillsText;
        [SerializeField] private TextMeshProUGUI _completeTimeText;
        [SerializeField] private Button _levelCompleteRestartButton;
        [SerializeField] private Button _levelCompleteQuitButton;
        #endregion

        #region Unity Lifecycle
        private void OnEnable()
        {
            // Subscribe to events
            PlayerHealth.OnHealthChanged += UpdateHealth;
            PlayerHealth.OnDamageTaken += ShowDamageDirection;
            WeaponManager.OnAmmoChanged += UpdateAmmo;
            GameManager.OnScoreChanged += UpdateScore;
            GameManager.OnGamePaused += ShowPauseMenu;
            GameManager.OnGameResumed += HidePauseMenu;
            GameManager.OnGameOver += ShowGameOver;
            GameManager.OnLevelComplete += ShowLevelComplete;
        }

        private void OnDisable()
        {
            // Unsubscribe from events
            PlayerHealth.OnHealthChanged -= UpdateHealth;
            PlayerHealth.OnDamageTaken -= ShowDamageDirection;
            WeaponManager.OnAmmoChanged -= UpdateAmmo;
            GameManager.OnScoreChanged -= UpdateScore;
            GameManager.OnGamePaused -= ShowPauseMenu;
            GameManager.OnGameResumed -= HidePauseMenu;
            GameManager.OnGameOver -= ShowGameOver;
            GameManager.OnLevelComplete -= ShowLevelComplete;
        }

        private void Start()
        {
            InitializeUI();
            SetupButtons();
        }

        private void Update()
        {
            UpdateDamageVignette();
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initialize UI state.
        /// </summary>
        private void InitializeUI()
        {
            if (_hudPanel != null) _hudPanel.SetActive(true);
            if (_pauseMenuPanel != null) _pauseMenuPanel.SetActive(false);
            if (_gameOverPanel != null) _gameOverPanel.SetActive(false);
            if (_levelCompletePanel != null) _levelCompletePanel.SetActive(false);

            UpdateScore(0);
        }

        /// <summary>
        /// Setup button listeners.
        /// </summary>
        private void SetupButtons()
        {
            if (_resumeButton != null)
                _resumeButton.onClick.AddListener(() => GameManager.Instance.ResumeGame());

            if (_restartButton != null)
                _restartButton.onClick.AddListener(() => GameManager.Instance.RestartLevel());

            if (_quitButton != null)
                _quitButton.onClick.AddListener(() => GameManager.Instance.QuitGame());

            if (_gameOverRestartButton != null)
                _gameOverRestartButton.onClick.AddListener(() => GameManager.Instance.RestartLevel());

            if (_gameOverQuitButton != null)
                _gameOverQuitButton.onClick.AddListener(() => GameManager.Instance.QuitGame());

            if (_levelCompleteRestartButton != null)
                _levelCompleteRestartButton.onClick.AddListener(() => GameManager.Instance.RestartLevel());

            if (_levelCompleteQuitButton != null)
                _levelCompleteQuitButton.onClick.AddListener(() => GameManager.Instance.QuitGame());
        }
        #endregion

        #region HUD Updates
        /// <summary>
        /// Update health display.
        /// </summary>
        /// <param name="current">Current health</param>
        /// <param name="max">Max health</param>
        private void UpdateHealth(float current, float max)
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

        /// <summary>
        /// Update ammo display.
        /// </summary>
        /// <param name="current">Current ammo in magazine</param>
        /// <param name="reserve">Reserve ammo</param>
        private void UpdateAmmo(int current, int reserve)
        {
            if (_ammoText != null)
            {
                _ammoText.text = $"{current} / {reserve}";
            }
        }

        /// <summary>
        /// Update score display.
        /// </summary>
        /// <param name="score">Current score</param>
        private void UpdateScore(int score)
        {
            if (_scoreText != null)
            {
                _scoreText.text = $"Score: {score}";
            }
        }

        /// <summary>
        /// Show damage direction indicator.
        /// </summary>
        /// <param name="damageSource">Position of damage source</param>
        private void ShowDamageDirection(Vector3 damageSource)
        {
            // Simple red flash for now
            if (_damageVignette != null)
            {
                Color color = _damageVignette.color;
                color.a = 0.5f;
                _damageVignette.color = color;
            }
        }

        /// <summary>
        /// Update damage vignette fade.
        /// </summary>
        private void UpdateDamageVignette()
        {
            if (_damageVignette != null)
            {
                Color color = _damageVignette.color;
                if (color.a > 0f)
                {
                    color.a -= Time.deltaTime * 2f;
                    _damageVignette.color = color;
                }
            }
        }
        #endregion

        #region Menu Displays
        /// <summary>
        /// Show pause menu.
        /// </summary>
        private void ShowPauseMenu()
        {
            if (_pauseMenuPanel != null)
            {
                _pauseMenuPanel.SetActive(true);
            }

            if (_hudPanel != null)
            {
                _hudPanel.SetActive(false);
            }
        }

        /// <summary>
        /// Hide pause menu.
        /// </summary>
        private void HidePauseMenu()
        {
            if (_pauseMenuPanel != null)
            {
                _pauseMenuPanel.SetActive(false);
            }

            if (_hudPanel != null)
            {
                _hudPanel.SetActive(true);
            }
        }

        /// <summary>
        /// Show game over screen.
        /// </summary>
        private void ShowGameOver()
        {
            if (_gameOverPanel != null)
            {
                _gameOverPanel.SetActive(true);
            }

            if (_hudPanel != null)
            {
                _hudPanel.SetActive(false);
            }

            UpdateGameOverStats();
        }

        /// <summary>
        /// Show level complete screen.
        /// </summary>
        private void ShowLevelComplete()
        {
            if (_levelCompletePanel != null)
            {
                _levelCompletePanel.SetActive(true);
            }

            if (_hudPanel != null)
            {
                _hudPanel.SetActive(false);
            }

            UpdateLevelCompleteStats();
        }

        /// <summary>
        /// Update game over statistics.
        /// </summary>
        private void UpdateGameOverStats()
        {
            if (_finalScoreText != null)
            {
                _finalScoreText.text = $"Final Score: {GameManager.Instance.CurrentScore}";
            }

            if (_killsText != null)
            {
                _killsText.text = $"Kills: {GameManager.Instance.EnemiesKilled}";
            }

            if (_timeText != null)
            {
                int minutes = Mathf.FloorToInt(GameManager.Instance.GameTime / 60f);
                int seconds = Mathf.FloorToInt(GameManager.Instance.GameTime % 60f);
                _timeText.text = $"Time: {minutes:00}:{seconds:00}";
            }
        }

        /// <summary>
        /// Update level complete statistics.
        /// </summary>
        private void UpdateLevelCompleteStats()
        {
            if (_completeScoreText != null)
            {
                _completeScoreText.text = $"Score: {GameManager.Instance.CurrentScore}";
            }

            if (_completeKillsText != null)
            {
                _completeKillsText.text = $"Kills: {GameManager.Instance.EnemiesKilled}";
            }

            if (_completeTimeText != null)
            {
                int minutes = Mathf.FloorToInt(GameManager.Instance.GameTime / 60f);
                int seconds = Mathf.FloorToInt(GameManager.Instance.GameTime % 60f);
                _completeTimeText.text = $"Time: {minutes:00}:{seconds:00}";
            }
        }
        #endregion
    }
}
