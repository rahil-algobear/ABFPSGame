using UnityEngine;

namespace Game.Pickups
{
    /// <summary>
    /// Base class for all pickups.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public abstract class PickupBase : MonoBehaviour
    {
        #region Settings
        [Header("Pickup Settings")]
        [SerializeField] protected bool _rotatePickup = true;
        [SerializeField] protected float _rotationSpeed = 90f;
        [SerializeField] protected bool _bobPickup = true;
        [SerializeField] protected float _bobSpeed = 2f;
        [SerializeField] protected float _bobAmount = 0.3f;
        [SerializeField] protected bool _respawn = false;
        [SerializeField] protected float _respawnTime = 30f;
        #endregion

        #region Components
        protected Collider _collider;
        protected Renderer _renderer;
        private Vector3 _startPosition;
        private float _bobTimer;
        #endregion

        #region Unity Lifecycle
        protected virtual void Awake()
        {
            _collider = GetComponent<Collider>();
            _collider.isTrigger = true;

            _renderer = GetComponentInChildren<Renderer>();
            _startPosition = transform.position;
        }

        protected virtual void Update()
        {
            if (_rotatePickup)
            {
                transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime);
            }

            if (_bobPickup)
            {
                _bobTimer += Time.deltaTime * _bobSpeed;
                float newY = _startPosition.y + Mathf.Sin(_bobTimer) * _bobAmount;
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            }
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (OnPickup(other.gameObject))
                {
                    if (_respawn)
                    {
                        StartCoroutine(RespawnCoroutine());
                    }
                    else
                    {
                        Destroy(gameObject);
                    }
                }
            }
        }
        #endregion

        #region Abstract Methods
        /// <summary>
        /// Called when player picks up the item.
        /// </summary>
        /// <param name="player">Player GameObject</param>
        /// <returns>True if pickup was successful</returns>
        protected abstract bool OnPickup(GameObject player);
        #endregion

        #region Respawn
        /// <summary>
        /// Respawn coroutine.
        /// </summary>
        protected System.Collections.IEnumerator RespawnCoroutine()
        {
            // Hide pickup
            if (_renderer != null)
            {
                _renderer.enabled = false;
            }
            _collider.enabled = false;

            yield return new WaitForSeconds(_respawnTime);

            // Show pickup
            if (_renderer != null)
            {
                _renderer.enabled = true;
            }
            _collider.enabled = true;
        }
        #endregion
    }
}
