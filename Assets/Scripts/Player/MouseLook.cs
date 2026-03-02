using UnityEngine;

namespace Game.Player
{
    /// <summary>
    /// First-person mouse look controller with recoil support.
    /// </summary>
    public class MouseLook : MonoBehaviour
    {
        #region Settings
        [Header("Mouse Settings")]
        [SerializeField] private float _mouseSensitivity = 2f;
        [SerializeField] private float _verticalClampMin = -90f;
        [SerializeField] private float _verticalClampMax = 90f;
        [SerializeField] private bool _invertY = false;

        [Header("Smoothing")]
        [SerializeField] private bool _enableSmoothing = true;
        [SerializeField] private float _smoothingAmount = 5f;
        #endregion

        #region Recoil
        [Header("Recoil")]
        [SerializeField] private float _recoilRecoverySpeed = 5f;
        private Vector2 _currentRecoil;
        private Vector2 _targetRecoil;
        #endregion

        #region Rotation
        private float _rotationX = 0f;
        private float _rotationY = 0f;
        private Vector2 _currentMouseDelta;
        private Vector2 _smoothMouseDelta;
        #endregion

        #region Components
        [Header("Components")]
        [SerializeField] private Transform _playerBody;
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (_playerBody == null)
            {
                _playerBody = transform.parent;
            }
        }

        private void Update()
        {
            HandleMouseLook();
            HandleRecoilRecovery();
        }
        #endregion

        #region Mouse Look
        /// <summary>
        /// Handle mouse look input and rotation.
        /// </summary>
        private void HandleMouseLook()
        {
            // Get mouse input
            float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity;

            if (_invertY)
            {
                mouseY = -mouseY;
            }

            // Apply smoothing
            if (_enableSmoothing)
            {
                _currentMouseDelta = new Vector2(mouseX, mouseY);
                _smoothMouseDelta = Vector2.Lerp(_smoothMouseDelta, _currentMouseDelta, Time.deltaTime * _smoothingAmount);
                mouseX = _smoothMouseDelta.x;
                mouseY = _smoothMouseDelta.y;
            }

            // Apply recoil
            mouseX += _currentRecoil.x;
            mouseY += _currentRecoil.y;

            // Calculate rotation
            _rotationX -= mouseY;
            _rotationX = Mathf.Clamp(_rotationX, _verticalClampMin, _verticalClampMax);
            _rotationY += mouseX;

            // Apply rotation
            transform.localRotation = Quaternion.Euler(_rotationX, 0f, 0f);

            if (_playerBody != null)
            {
                _playerBody.localRotation = Quaternion.Euler(0f, _rotationY, 0f);
            }
        }
        #endregion

        #region Recoil
        /// <summary>
        /// Apply recoil to camera.
        /// </summary>
        public void ApplyRecoil(float horizontal, float vertical)
        {
            _targetRecoil += new Vector2(horizontal, vertical);
        }

        /// <summary>
        /// Handle recoil recovery over time.
        /// </summary>
        private void HandleRecoilRecovery()
        {
            // Smoothly move current recoil towards target
            _currentRecoil = Vector2.Lerp(_currentRecoil, _targetRecoil, Time.deltaTime * 10f);

            // Recover recoil back to zero
            _targetRecoil = Vector2.Lerp(_targetRecoil, Vector2.zero, Time.deltaTime * _recoilRecoverySpeed);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Set mouse sensitivity.
        /// </summary>
        public void SetSensitivity(float sensitivity)
        {
            _mouseSensitivity = sensitivity;
        }

        /// <summary>
        /// Toggle Y-axis inversion.
        /// </summary>
        public void SetInvertY(bool invert)
        {
            _invertY = invert;
        }
        #endregion
    }
}
