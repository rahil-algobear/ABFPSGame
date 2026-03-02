using UnityEngine;
using System.Collections.Generic;
using Game.Core;

namespace Game.Enemies
{
    public class SpawnManager : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private GameObject[] _enemyPrefabs;
        [SerializeField] private Transform[] _spawnPoints;
        [SerializeField] private int _maxEnemies = 10;
        [SerializeField] private float _spawnInterval = 5f;
        [SerializeField] private int _totalEnemiesToSpawn = 20;

        private List<GameObject> _activeEnemies = new List<GameObject>();
        private int _enemiesSpawned = 0;
        private float _lastSpawnTime;

        private void Start()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetTotalEnemies(_totalEnemiesToSpawn);
            }
        }

        private void Update()
        {
            CleanupDeadEnemies();

            if (_enemiesSpawned < _totalEnemiesToSpawn && 
                _activeEnemies.Count < _maxEnemies && 
                Time.time >= _lastSpawnTime + _spawnInterval)
            {
                SpawnEnemy();
            }
        }

        private void SpawnEnemy()
        {
            if (_enemyPrefabs.Length == 0 || _spawnPoints.Length == 0)
                return;

            GameObject enemyPrefab = _enemyPrefabs[Random.Range(0, _enemyPrefabs.Length)];
            Transform spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];

            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            _activeEnemies.Add(enemy);
            _enemiesSpawned++;
            _lastSpawnTime = Time.time;
        }

        private void CleanupDeadEnemies()
        {
            _activeEnemies.RemoveAll(enemy => enemy == null || enemy.GetComponent<EnemyBase>()?.IsDead == true);
        }
    }
}