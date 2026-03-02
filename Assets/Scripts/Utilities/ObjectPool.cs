using UnityEngine;
using System.Collections.Generic;

namespace Game.Utilities
{
    /// <summary>
    /// Generic object pooling system for performance optimization.
    /// </summary>
    public class ObjectPool : MonoBehaviour
    {
        #region Pool Settings
        [Header("Pool Settings")]
        [SerializeField] private GameObject _prefab;
        [SerializeField] private int _initialSize = 10;
        [SerializeField] private bool _expandable = true;
        #endregion

        #region Pool
        private Queue<GameObject> _pool = new Queue<GameObject>();
        private List<GameObject> _activeObjects = new List<GameObject>();
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            InitializePool();
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initialize object pool with initial size.
        /// </summary>
        private void InitializePool()
        {
            for (int i = 0; i < _initialSize; i++)
            {
                CreateNewObject();
            }
        }

        /// <summary>
        /// Create a new pooled object.
        /// </summary>
        private GameObject CreateNewObject()
        {
            GameObject obj = Instantiate(_prefab, transform);
            obj.SetActive(false);
            _pool.Enqueue(obj);
            return obj;
        }
        #endregion

        #region Pool Operations
        /// <summary>
        /// Get an object from the pool.
        /// </summary>
        public GameObject GetObject()
        {
            GameObject obj;

            if (_pool.Count > 0)
            {
                obj = _pool.Dequeue();
            }
            else if (_expandable)
            {
                obj = CreateNewObject();
            }
            else
            {
                return null;
            }

            obj.SetActive(true);
            _activeObjects.Add(obj);
            return obj;
        }

        /// <summary>
        /// Return an object to the pool.
        /// </summary>
        public void ReturnObject(GameObject obj)
        {
            if (obj == null) return;

            obj.SetActive(false);
            obj.transform.SetParent(transform);
            _activeObjects.Remove(obj);
            _pool.Enqueue(obj);
        }

        /// <summary>
        /// Return all active objects to pool.
        /// </summary>
        public void ReturnAllObjects()
        {
            for (int i = _activeObjects.Count - 1; i >= 0; i--)
            {
                ReturnObject(_activeObjects[i]);
            }
        }
        #endregion
    }
}
