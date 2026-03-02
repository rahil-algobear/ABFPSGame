using UnityEngine;
using System.Collections.Generic;
using Game.Core;

namespace Game.Enemies
{
    /// <summary>
    /// Manages enemy spawning and wave progression.
    /// </summary>
    public class SpawnManager : MonoBehaviour
    {
        #region Spawn Settings
        [Header("Spawn Settings")]
        [SerializeField] private GameObject[] _enemyPrefabs;
        [SerializeField] private Transform[] _spawnPoints;
        [SerializeField] private int _initialEnemyCount = 10;
        [SerializeField] private float _spawnDelay = 2f;
        #endregion

        #region Wave Settings
        [Header("Wave Settings")]
        [SerializeField] private bool _enableWaves = false;
        [SerializeField] private int _enemiesPerWave = 5;
        [SerializeField] private float _waveCooldown = 10f;
        #endregion

        #region State
        private List<EnemyBase> _activeEnemies = new List<EnemyBase>();
        private int _currentWave = 0;
        private float _waveTimer = 0f;
        private bool _waveInProgress = false;
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            SpawnInitialEnemies();
        }

        private void Update()
        {
            if (_enableWaves)
            {
                UpdateWaveSystem();
            }
        }
        #endregion

        #region Initial Spawn
        /// <summary>
        /// Spawns the initial set of enemies.
        /// </summary>
        private void SpawnInitialEnemies()
        {
            if (_enemyPrefabs == null || _enemyPrefabs.Length == 0)
            {
                Debug.LogWarning("No enemy prefabs assigned to SpawnManager!");
                return;
            }

            if (_spawnPoints == null || _spawnPoints.Length == 0)
            {
                Debug.LogWarning("No spawn points assigned to SpawnManager!");
                return;
            }

            for (int i = 0; i < _initialEnemyCount; i++)
            {
                SpawnEnemy();
            }

            // Set total enemies for game manager
            GameManager.Instance.SetTotalEnemies(_initialEnemyCount);
        }
        #endregion

        #region Wave System
        /// <summary>
        /// Updates the wave spawning system.
        /// </summary>
        private void UpdateWaveSystem()
        {
            // Check if all enemies are dead
            _activeEnemies.RemoveAll(enemy => enemy == null || enemy.IsDead());

            if (_activeEnemies.Count == 0 && !_waveInProgress)
            {
                _waveTimer += Time.deltaTime;

                if (_waveTimer >= _waveCooldown)
                {
                    _waveTimer = 0f;
                    StartNextWave();
                }
            }
        }

        /// <summary>
        /// Starts the next wave of enemies.
        /// </summary>
        private void StartNextWave()
        {
            _currentWave++;
            _waveInProgress = true;

            int enemiesToSpawn = _enemiesPerWave + (_currentWave * 2); // Increase difficulty

            for (int i = 0; i < enemiesToSpawn; i++)
            {
                SpawnEnemy();
            }

            GameManager.Instance.SetTotalEnemies(GameManager.Instance.TotalEnemies + enemiesToSpawn);
            _waveInProgress = false;
        }
        #endregion

        #region Spawning
        /// <summary>
        /// Spawns a random enemy at a random spawn point.
        /// </summary>
        private void SpawnEnemy()
        {
            if (_enemyPrefabs.Length == 0 || _spawnPoints.Length == 0)
                return;

            // Select random enemy and spawn point
            GameObject enemyPrefab = _enemyPrefabs[Random.Range(0, _enemyPrefabs.Length)];
            Transform spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];

            // Spawn enemy
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            EnemyBase enemyBase = enemy.GetComponent<EnemyBase>();

            if (enemyBase != null)
            {
                _activeEnemies.Add(enemyBase);
            }
        }

        /// <summary>
        /// Spawns a specific enemy type at a specific location.
        /// </summary>
        public void SpawnEnemyAt(int enemyIndex, Vector3 position)
        {
            if (enemyIndex < 0 || enemyIndex >= _enemyPrefabs.Length)
                return;

            GameObject enemy = Instantiate(_enemyPrefabs[enemyIndex], position, Quaternion.identity);
            EnemyBase enemyBase = enemy.GetComponent<EnemyBase>();

            if (enemyBase != null)
            {
                _activeEnemies.Add(enemyBase);
            }
        }
        #endregion

        #region Enemy Registration
        /// <summary>
        /// Registers an enemy with the spawn manager.
        /// </summary>
        public void RegisterEnemy(EnemyBase enemy)
        {
            if (!_activeEnemies.Contains(enemy))
            {
                _activeEnemies.Add(enemy);
            }
        }

        /// <summary>
        /// Unregisters an enemy from the spawn manager.
        /// </summary>
        public void UnregisterEnemy(EnemyBase enemy)
        {
            _activeEnemies.Remove(enemy);
        }
        #endregion

        #region Public Getters
        public int GetActiveEnemyCount() => _activeEnemies.Count;
        public int GetCurrentWave() => _currentWave;
        #endregion
    }
}