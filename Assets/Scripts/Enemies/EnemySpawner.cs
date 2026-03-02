using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Game.Enemies
{
    /// <summary>
    /// Spawns enemies in waves or at trigger points.
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        #region Spawn Settings
        [Header("Spawn Settings")]
        [SerializeField] private GameObject[] _enemyPrefabs;
        [SerializeField] private Transform[] _spawnPoints;
        [SerializeField] private int _maxActiveEnemies = 10;
        #endregion

        #region Wave Settings
        [Header("Wave Settings")]
        [SerializeField] private bool _useWaves = true;
        [SerializeField] private int _enemiesPerWave = 5;
        [SerializeField] private float _timeBetweenWaves = 10f;
        [SerializeField] private int _maxWaves = 4;

        private int _currentWave = 0;
        private float _waveTimer;
        #endregion

        #region Active Enemies
        private List<GameObject> _activeEnemies = new List<GameObject>();
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            if (_useWaves)
            {
                StartCoroutine(WaveSpawner());
            }
        }

        private void Update()
        {
            // Clean up destroyed enemies
            _activeEnemies.RemoveAll(enemy => enemy == null);
        }
        #endregion

        #region Wave Spawning
        /// <summary>
        /// Wave-based spawning coroutine.
        /// </summary>
        private IEnumerator WaveSpawner()
        {
            while (_currentWave < _maxWaves)
            {
                yield return new WaitForSeconds(_timeBetweenWaves);

                SpawnWave();
                _currentWave++;
            }
        }

        /// <summary>
        /// Spawn a wave of enemies.
        /// </summary>
        private void SpawnWave()
        {
            int enemiesToSpawn = Mathf.Min(_enemiesPerWave, _maxActiveEnemies - _activeEnemies.Count);

            for (int i = 0; i < enemiesToSpawn; i++)
            {
                SpawnEnemy();
            }
        }
        #endregion

        #region Enemy Spawning
        /// <summary>
        /// Spawn a single enemy at a random spawn point.
        /// </summary>
        public void SpawnEnemy()
        {
            if (_enemyPrefabs == null || _enemyPrefabs.Length == 0) return;
            if (_spawnPoints == null || _spawnPoints.Length == 0) return;
            if (_activeEnemies.Count >= _maxActiveEnemies) return;

            // Select random enemy and spawn point
            GameObject enemyPrefab = _enemyPrefabs[Random.Range(0, _enemyPrefabs.Length)];
            Transform spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];

            // Spawn enemy
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            _activeEnemies.Add(enemy);
        }

        /// <summary>
        /// Spawn enemy at specific position.
        /// </summary>
        /// <param name="position">Spawn position</param>
        public void SpawnEnemyAtPosition(Vector3 position)
        {
            if (_enemyPrefabs == null || _enemyPrefabs.Length == 0) return;
            if (_activeEnemies.Count >= _maxActiveEnemies) return;

            GameObject enemyPrefab = _enemyPrefabs[Random.Range(0, _enemyPrefabs.Length)];
            GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
            _activeEnemies.Add(enemy);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get number of active enemies.
        /// </summary>
        /// <returns>Active enemy count</returns>
        public int GetActiveEnemyCount()
        {
            return _activeEnemies.Count;
        }
        #endregion
    }
}
