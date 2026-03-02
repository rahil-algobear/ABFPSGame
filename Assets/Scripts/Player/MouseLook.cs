using UnityEngine;
using Game.Core;

namespace Game.Player
{
    /// <summary>
    /// Smooth mouse look camera control for FPS.
    /// </summary>
    public class MouseLook : MonoBehaviour
    {
        #region Settings
        [Header("Mouse Settings")]
        [SerializeField] private float _mouseSensitivity = 100f;
        [SerializeField] private bool _invertY = false;

        [Header("Camera Limits")]
        [SerializeField] private float _minVerticalAngle = -90f;
        [SerializeField] private float _maxVerticalAngle = 90f;

        [Header("Smoothing")]
        [SerializeField] private bool _enableSmoothing = true;
        [SerializeField] private float _smoothTime = 0.1f;
        #endregion

        #region Components
        [Header("References")]
        [SerializeField] private Transform _playerBody;
        [SerializeField] private Camera _camera;
        #endregion

        #region Private Fields
        private float _xRotation = 0f;
        private Vector2 _currentMouseDelta;
        private Vector2 _currentMouseDeltaVelocity;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            if (_camera == null)
            {
                _camera = GetComponent<Camera>();
            }

            if (_playerBody == null)
            {
                _playerBody = transform.parent;
            }
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (GameManager.Instance.IsPaused || GameManager.Instance.IsGameOver)
            {
                return;
            }

            HandleMouseLook();
        }
        #endregion

        #region Mouse Look
        /// <summary>
        /// Handle mouse look input and camera rotation.
        /// </summary>
        private void HandleMouseLook()
        {
            // Get mouse input
            float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity * Time.deltaTime;

            if (_invertY)
            {
                mouseY = -mouseY;
            }

            Vector2 targetMouseDelta = new Vector2(mouseX, mouseY);

            // Apply smoothing
            if (_enableSmoothing)
            {
                _currentMouseDelta = Vector2.SmoothDamp(
                    _currentMouseDelta,
                    targetMouseDelta,
                    ref _currentMouseDeltaVelocity,
                    _smoothTime
                );
            }
            else
            {
                _currentMouseDelta = targetMouseDelta;
            }

            // Rotate camera vertically
            _xRotation -= _currentMouseDelta.y;
            _xRotation = Mathf.Clamp(_xRotation, _minVerticalAngle, _maxVerticalAngle);
            transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

            // Rotate player body horizontally
            if (_playerBody != null)
            {
                _playerBody.Rotate(Vector3.up * _currentMouseDelta.x);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Set mouse sensitivity.
        /// </summary>
        /// <param name="sensitivity">New sensitivity value</param>
        public void SetSensitivity(float sensitivity)
        {
            _mouseSensitivity = Mathf.Max(0f, sensitivity);
        }

        /// <summary>
        /// Get current mouse sensitivity.
        /// </summary>
        /// <returns>Current sensitivity</returns>
        public float GetSensitivity()
        {
            return _mouseSensitivity;
        }

        /// <summary>
        /// Toggle Y-axis inversion.
        /// </summary>
        /// <param name="invert">True to invert Y-axis</param>
        public void SetInvertY(bool invert)
        {
            _invertY = invert;
        }

        /// <summary>
        /// Add camera recoil effect.
        /// </summary>
        /// <param name="recoilAmount">Recoil amount</param>
        public void AddRecoil(Vector2 recoilAmount)
        {
            _xRotation -= recoilAmount.y;
            _xRotation = Mathf.Clamp(_xRotation, _minVerticalAngle, _maxVerticalAngle);

            if (_playerBody != null)
            {
                _playerBody.Rotate(Vector3.up * recoilAmount.x);
            }
        }
        #endregion
    }
}
