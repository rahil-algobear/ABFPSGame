using UnityEngine;
using System.Collections;

namespace Game.Utilities
{
    /// <summary>
    /// Camera shake effect for impacts and explosions.
    /// </summary>
    public class CameraShake : MonoBehaviour
    {
        #region Singleton
        private static CameraShake _instance;
        public static CameraShake Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<CameraShake>();
                }
                return _instance;
            }
        }
        #endregion

        #region State
        private Vector3 _originalPosition;
        private bool _isShaking = false;
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
            _originalPosition = transform.localPosition;
        }
        #endregion

        #region Shake
        /// <summary>
        /// Trigger camera shake effect.
        /// </summary>
        public void Shake(float duration, float magnitude)
        {
            if (!_isShaking)
            {
                StartCoroutine(ShakeCoroutine(duration, magnitude));
            }
        }

        /// <summary>
        /// Camera shake coroutine.
        /// </summary>
        private IEnumerator ShakeCoroutine(float duration, float magnitude)
        {
            _isShaking = true;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;

                transform.localPosition = _originalPosition + new Vector3(x, y, 0f);

                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = _originalPosition;
            _isShaking = false;
        }
        #endregion
    }
}
