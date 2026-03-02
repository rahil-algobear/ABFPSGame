using UnityEngine;
using Game.Core;

namespace Game.Player
{
    /// <summary>
    /// CharacterController-based FPS player movement.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        #region Movement Settings
        [Header("Movement Settings")]
        [SerializeField] private float _walkSpeed = 5f;
        [SerializeField] private float _sprintSpeed = 8f;
        [SerializeField] private float _crouchSpeed = 2.5f;
        [SerializeField] private float _jumpHeight = 2f;
        [SerializeField] private float _gravity = -20f;
        #endregion

        #region Ground Check
        [Header("Ground Check")]
        [SerializeField] private Transform _groundCheck;
        [SerializeField] private float _groundDistance = 0.4f;
        [SerializeField] private LayerMask _groundMask;
        #endregion

        #region Head Bob
        [Header("Head Bob")]
        [SerializeField] private bool _enableHeadBob = true;
        [SerializeField] private float _bobSpeed = 14f;
        [SerializeField] private float _bobAmount = 0.05f;
        private float _defaultYPos;
        private float _timer;
        #endregion

        #region Footsteps
        [Header("Footsteps")]
        [SerializeField] private AudioClip[] _footstepSounds;
        [SerializeField] private float _footstepInterval = 0.5f;
        private float _footstepTimer;
        #endregion

        #region Components
        private CharacterController _controller;
        private Camera _camera;
        private Vector3 _velocity;
        private bool _isGrounded;
        private bool _isSprinting;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _camera = GetComponentInChildren<Camera>();

            if (_camera != null)
            {
                _defaultYPos = _camera.transform.localPosition.y;
            }

            // Create ground check if not assigned
            if (_groundCheck == null)
            {
                GameObject groundCheckObj = new GameObject("GroundCheck");
                groundCheckObj.transform.SetParent(transform);
                groundCheckObj.transform.localPosition = new Vector3(0, -_controller.height / 2f, 0);
                _groundCheck = groundCheckObj.transform;
            }
        }

        private void Update()
        {
            if (GameManager.Instance.IsPaused || GameManager.Instance.IsGameOver)
            {
                return;
            }

            HandleGroundCheck();
            HandleMovement();
            HandleHeadBob();
            HandleFootsteps();
        }
        #endregion

        #region Ground Check
        /// <summary>
        /// Check if player is grounded.
        /// </summary>
        private void HandleGroundCheck()
        {
            _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundDistance, _groundMask);

            if (_isGrounded && _velocity.y < 0)
            {
                _velocity.y = -2f; // Small downward force to keep grounded
            }
        }
        #endregion

        #region Movement
        /// <summary>
        /// Handle player movement input and physics.
        /// </summary>
        private void HandleMovement()
        {
            // Get input
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            // Calculate movement direction
            Vector3 move = transform.right * horizontal + transform.forward * vertical;

            // Determine speed
            _isSprinting = Input.GetKey(KeyCode.LeftShift) && vertical > 0;
            float currentSpeed = _isSprinting ? _sprintSpeed : _walkSpeed;

            // Apply movement
            _controller.Move(move * currentSpeed * Time.deltaTime);

            // Handle jump
            if (Input.GetButtonDown("Jump") && _isGrounded)
            {
                _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
            }

            // Apply gravity
            _velocity.y += _gravity * Time.deltaTime;
            _controller.Move(_velocity * Time.deltaTime);
        }
        #endregion

        #region Head Bob
        /// <summary>
        /// Apply head bob effect during movement.
        /// </summary>
        private void HandleHeadBob()
        {
            if (!_enableHeadBob || _camera == null) return;

            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            if (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f)
            {
                if (_isGrounded)
                {
                    _timer += Time.deltaTime * _bobSpeed;
                    float newY = _defaultYPos + Mathf.Sin(_timer) * _bobAmount;
                    _camera.transform.localPosition = new Vector3(
                        _camera.transform.localPosition.x,
                        newY,
                        _camera.transform.localPosition.z
                    );
                }
            }
            else
            {
                _timer = 0;
                // Smoothly return to default position
                Vector3 currentPos = _camera.transform.localPosition;
                currentPos.y = Mathf.Lerp(currentPos.y, _defaultYPos, Time.deltaTime * _bobSpeed);
                _camera.transform.localPosition = currentPos;
            }
        }
        #endregion

        #region Footsteps
        /// <summary>
        /// Play footstep sounds during movement.
        /// </summary>
        private void HandleFootsteps()
        {
            if (_footstepSounds == null || _footstepSounds.Length == 0) return;

            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            if (_isGrounded && (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f))
            {
                _footstepTimer += Time.deltaTime;

                float interval = _isSprinting ? _footstepInterval * 0.7f : _footstepInterval;

                if (_footstepTimer >= interval)
                {
                    _footstepTimer = 0f;
                    AudioClip clip = _footstepSounds[Random.Range(0, _footstepSounds.Length)];
                    AudioManager.Instance.PlaySpatialSFX(clip, transform.position, 0.5f);
                }
            }
            else
            {
                _footstepTimer = 0f;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get current movement speed.
        /// </summary>
        /// <returns>Current speed</returns>
        public float GetCurrentSpeed()
        {
            return _isSprinting ? _sprintSpeed : _walkSpeed;
        }

        /// <summary>
        /// Check if player is currently sprinting.
        /// </summary>
        /// <returns>True if sprinting</returns>
        public bool IsSprinting()
        {
            return _isSprinting;
        }

        /// <summary>
        /// Check if player is grounded.
        /// </summary>
        /// <returns>True if grounded</returns>
        public bool IsGrounded()
        {
            return _isGrounded;
        }
        #endregion
    }
}
