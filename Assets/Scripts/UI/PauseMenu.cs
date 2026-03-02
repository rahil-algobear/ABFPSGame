using UnityEngine;
using UnityEngine.UI;
using Game.Core;

namespace Game.UI
{
    /// <summary>
    /// Manages the pause menu UI.
    /// </summary>
    public class PauseMenu : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject _pauseMenuPanel;
        [SerializeField] private GameObject _gameOverPanel;
        [SerializeField] private GameObject _levelCompletePanel;

        [Header("Buttons")]
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _quitButton;

        private void Start()
        {
            // Setup button listeners
            if (_resumeButton != null)
                _resumeButton.onClick.AddListener(ResumeGame);
            if (_restartButton != null)
                _restartButton.onClick.AddListener(RestartGame);
            if (_quitButton != null)
                _quitButton.onClick.AddListener(QuitGame);

            // Subscribe to game events
            GameManager.OnGameOver += ShowGameOver;
            GameManager.OnLevelComplete += ShowLevelComplete;

            // Hide all panels
            HideAllPanels();
        }

        private void OnDestroy()
        {
            GameManager.OnGameOver -= ShowGameOver;
            GameManager.OnLevelComplete -= ShowLevelComplete;
        }

        private void Update()
        {
            // Toggle pause menu with ESC
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (GameManager.Instance.IsPaused)
                {
                    ResumeGame();
                }
                else if (!GameManager.Instance.IsGameOver)
                {
                    PauseGame();
                }
            }
        }

        /// <summary>
        /// Pauses the game and shows the pause menu.
        /// </summary>
        public void PauseGame()
        {
            GameManager.Instance.PauseGame();
            if (_pauseMenuPanel != null)
                _pauseMenuPanel.SetActive(true);
        }

        /// <summary>
        /// Resumes the game and hides the pause menu.
        /// </summary>
        public void ResumeGame()
        {
            GameManager.Instance.ResumeGame();
            HideAllPanels();
        }

        /// <summary>
        /// Restarts the current level.
        /// </summary>
        public void RestartGame()
        {
            GameManager.Instance.RestartLevel();
        }

        /// <summary>
        /// Quits the game.
        /// </summary>
        public void QuitGame()
        {
            GameManager.Instance.QuitGame();
        }

        /// <summary>
        /// Shows the game over screen.
        /// </summary>
        private void ShowGameOver()
        {
            HideAllPanels();
            if (_gameOverPanel != null)
                _gameOverPanel.SetActive(true);
        }

        /// <summary>
        /// Shows the level complete screen.
        /// </summary>
        private void ShowLevelComplete()
        {
            HideAllPanels();
            if (_levelCompletePanel != null)
                _levelCompletePanel.SetActive(true);
        }

        /// <summary>
        /// Hides all menu panels.
        /// </summary>
        private void HideAllPanels()
        {
            if (_pauseMenuPanel != null)
                _pauseMenuPanel.SetActive(false);
            if (_gameOverPanel != null)
                _gameOverPanel.SetActive(false);
            if (_levelCompletePanel != null)
                _levelCompletePanel.SetActive(false);
        }
    }
}