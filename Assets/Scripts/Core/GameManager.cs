using UnityEngine;
using UnityEngine.SceneManagement;
using System;

namespace Game.Core
{
    /// <summary>
    /// Singleton GameManager that persists across scenes and manages global game state.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        #region Singleton
        private static GameManager _instance;
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<GameManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("GameManager");
                        _instance = go.AddComponent<GameManager>();
                    }
                }
                return _instance;
            }
        }
        #endregion

        #region Events
        public static event Action<int> OnScoreChanged;
        public static event Action<int> OnEnemyKilled;
        public static event Action OnGamePaused;
        public static event Action OnGameResumed;
        public static event Action OnLevelComplete;
        public static event Action OnGameOver;
        #endregion

        #region Game State
        [Header("Game State")]
        [SerializeField] private bool _isPaused = false;
        [SerializeField] private bool _isGameOver = false;
        [SerializeField] private int _currentScore = 0;
        [SerializeField] private int _enemiesKilled = 0;
        [SerializeField] private float _gameTime = 0f;

        public bool IsPaused => _isPaused;
        public bool IsGameOver => _isGameOver;
        public int CurrentScore => _currentScore;
        public int EnemiesKilled => _enemiesKilled;
        public float GameTime => _gameTime;
        #endregion

        #region Level Settings
        [Header("Level Settings")]
        [SerializeField] private int _totalEnemiesInLevel = 20;
        [SerializeField] private int _enemiesRemainingToComplete = 0;

        public int TotalEnemiesInLevel => _totalEnemiesInLevel;
        public int EnemiesRemainingToComplete => _enemiesRemainingToComplete;
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
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            InitializeGame();
        }

        private void Update()
        {
            if (!_isPaused && !_isGameOver)
            {
                _gameTime += Time.deltaTime;
            }

            // Handle pause input
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initialize game state at start.
        /// </summary>
        private void InitializeGame()
        {
            _currentScore = 0;
            _enemiesKilled = 0;
            _gameTime = 0f;
            _isGameOver = false;
            _isPaused = false;
            _enemiesRemainingToComplete = _totalEnemiesInLevel;

            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Add score to the current total.
        /// </summary>
        /// <param name="points">Points to add</param>
        public void AddScore(int points)
        {
            _currentScore += points;
            OnScoreChanged?.Invoke(_currentScore);
        }

        /// <summary>
        /// Register an enemy kill.
        /// </summary>
        /// <param name="scoreValue">Score value for this kill</param>
        public void RegisterEnemyKill(int scoreValue = 100)
        {
            _enemiesKilled++;
            _enemiesRemainingToComplete--;
            AddScore(scoreValue);
            OnEnemyKilled?.Invoke(_enemiesKilled);

            // Check for level completion
            if (_enemiesRemainingToComplete <= 0)
            {
                CompleteLevel();
            }
        }

        /// <summary>
        /// Toggle pause state.
        /// </summary>
        public void TogglePause()
        {
            if (_isGameOver) return;

            _isPaused = !_isPaused;

            if (_isPaused)
            {
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                OnGamePaused?.Invoke();
            }
            else
            {
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                OnGameResumed?.Invoke();
            }
        }

        /// <summary>
        /// Pause the game.
        /// </summary>
        public void PauseGame()
        {
            if (!_isPaused)
            {
                TogglePause();
            }
        }

        /// <summary>
        /// Resume the game.
        /// </summary>
        public void ResumeGame()
        {
            if (_isPaused)
            {
                TogglePause();
            }
        }

        /// <summary>
        /// Trigger game over state.
        /// </summary>
        public void GameOver()
        {
            if (_isGameOver) return;

            _isGameOver = true;
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            OnGameOver?.Invoke();
        }

        /// <summary>
        /// Complete the current level.
        /// </summary>
        public void CompleteLevel()
        {
            if (_isGameOver) return;

            _isGameOver = true;
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            OnLevelComplete?.Invoke();
        }

        /// <summary>
        /// Restart the current level.
        /// </summary>
        public void RestartLevel()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            InitializeGame();
        }

        /// <summary>
        /// Quit the game.
        /// </summary>
        public void QuitGame()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
        #endregion
    }
}
