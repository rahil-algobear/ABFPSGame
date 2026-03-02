using UnityEngine;
using System.Collections.Generic;

namespace ABFPSGame.Core
{
    /// <summary>
    /// Centralized object pooling manager for frequently instantiated objects.
    /// </summary>
    public class ObjectPoolManager : MonoBehaviour
    {
        #region Singleton
        private static ObjectPoolManager _instance;
        public static ObjectPoolManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<ObjectPoolManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("ObjectPoolManager");
                        _instance = go.AddComponent<ObjectPoolManager>();
                    }
                }
                return _instance;
            }
        }
        #endregion

        #region Pool Data
        private Dictionary<string, Queue<GameObject>> _pools = new Dictionary<string, Queue<GameObject>>();
        private Dictionary<string, GameObject> _prefabs = new Dictionary<string, GameObject>();
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
        #endregion

        #region Pool Management
        /// <summary>
        /// Create a new pool for a prefab.
        /// </summary>
        /// <param name="poolName">Name of the pool</param>
        /// <param name="prefab">Prefab to pool</param>
        /// <param name="initialSize">Initial pool size</param>
        public void CreatePool(string poolName, GameObject prefab, int initialSize = 10)
        {
            if (_pools.ContainsKey(poolName))
            {
                Debug.LogWarning($"Pool '{poolName}' already exists!");
                return;
            }

            _prefabs[poolName] = prefab;
            _pools[poolName] = new Queue<GameObject>();

            GameObject poolContainer = new GameObject($"Pool_{poolName}");
            poolContainer.transform.SetParent(transform);

            for (int i = 0; i < initialSize; i++)
            {
                GameObject obj = Instantiate(prefab, poolContainer.transform);
                obj.SetActive(false);
                _pools[poolName].Enqueue(obj);
            }
        }

        /// <summary>
        /// Get an object from the pool.
        /// </summary>
        /// <param name="poolName">Name of the pool</param>
        /// <param name="position">Spawn position</param>
        /// <param name="rotation">Spawn rotation</param>
        /// <returns>Pooled GameObject</returns>
        public GameObject GetFromPool(string poolName, Vector3 position, Quaternion rotation)
        {
            if (!_pools.ContainsKey(poolName))
            {
                Debug.LogError($"Pool '{poolName}' does not exist!");
                return null;
            }

            GameObject obj;

            if (_pools[poolName].Count > 0)
            {
                obj = _pools[poolName].Dequeue();
            }
            else
            {
                // Expand pool if empty
                obj = Instantiate(_prefabs[poolName]);
            }

            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);

            return obj;
        }

        /// <summary>
        /// Return an object to the pool.
        /// </summary>
        /// <param name="poolName">Name of the pool</param>
        /// <param name="obj">Object to return</param>
        public void ReturnToPool(string poolName, GameObject obj)
        {
            if (!_pools.ContainsKey(poolName))
            {
                Debug.LogError($"Pool '{poolName}' does not exist!");
                Destroy(obj);
                return;
            }

            obj.SetActive(false);
            _pools[poolName].Enqueue(obj);
        }

        /// <summary>
        /// Clear all pools.
        /// </summary>
        public void ClearAllPools()
        {
            foreach (var pool in _pools.Values)
            {
                while (pool.Count > 0)
                {
                    GameObject obj = pool.Dequeue();
                    if (obj != null)
                    {
                        Destroy(obj);
                    }
                }
            }

            _pools.Clear();
            _prefabs.Clear();
        }
        #endregion
    }
}
