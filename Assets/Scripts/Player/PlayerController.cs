using UnityEngine;

namespace Game.Player
{
    /// <summary>
    /// First-person character controller with movement, jumping, and crouching.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        #region Movement Settings
        [Header("Movement")]
        [SerializeField] private float _walkSpeed = 5f;
        [SerializeField] private float _sprintSpeed = 8f;
        [SerializeField] private float _crouchSpeed = 2.5f;
        [SerializeField] private float _acceleration = 10f;
        #endregion

        #region Jump Settings
        [Header("Jump")]
        [SerializeField] private float _jumpHeight = 2f;
        [SerializeField] private float _gravity = -20f;
        #endregion

        #region Crouch Settings
        [Header("Crouch")]
        [SerializeField] private float _standingHeight = 2f;
        [SerializeField] private float _crouchHeight = 1f;
        [SerializeField] private float _crouchTransitionSpeed = 10f;
        #endregion

        #region Ground Check
        [Header("Ground Check")]
        [SerializeField] private Transform _groundCheck;
        [SerializeField] private float _groundDistance = 0.4f;
        [SerializeField] private LayerMask _groundMask;
        #endregion

        #region Components
        private CharacterController _controller;
        private Vector3 _velocity;
        private bool _isGrounded;
        private bool _isCrouching;
        private bool _isSprinting;
        private Vector3 _moveInput;
        #endregion

        #region Properties
        public bool IsGrounded => _isGrounded;
        public bool IsCrouching => _isCrouching;
        public bool IsSprinting => _isSprinting;
        public bool IsMoving => _moveInput.magnitude > 0.1f && _isGrounded;
        public Vector3 Velocity => _controller.velocity;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            _controller = GetComponent<CharacterController>();

            if (_groundCheck == null)
            {
                GameObject groundCheckObj = new GameObject("GroundCheck");
                groundCheckObj.transform.SetParent(transform);
                groundCheckObj.transform.localPosition = new Vector3(0f, -_controller.height / 2f, 0f);
                _groundCheck = groundCheckObj.transform;
            }
        }

        private void Update()
        {
            HandleGroundCheck();
            HandleMovement();
            HandleJump();
            HandleCrouch();
            ApplyGravity();
        }
        #endregion

        #region Ground Check
        private void HandleGroundCheck()
        {
            _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundDistance, _groundMask);

            if (_isGrounded && _velocity.y < 0f)
            {
                _velocity.y = -2f;
            }
        }
        #endregion

        #region Movement
        private void HandleMovement()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            _moveInput = transform.right * horizontal + transform.forward * vertical;
            _moveInput = Vector3.ClampMagnitude(_moveInput, 1f);

            _isSprinting = Input.GetKey(KeyCode.LeftShift) && !_isCrouching && _moveInput.z > 0f;

            float targetSpeed = _isCrouching ? _crouchSpeed : (_isSprinting ? _sprintSpeed : _walkSpeed);
            Vector3 targetVelocity = _moveInput * targetSpeed;

            Vector3 currentHorizontalVelocity = new Vector3(_controller.velocity.x, 0f, _controller.velocity.z);
            Vector3 newHorizontalVelocity = Vector3.Lerp(currentHorizontalVelocity, targetVelocity, _acceleration * Time.deltaTime);

            _controller.Move(newHorizontalVelocity * Time.deltaTime);
        }
        #endregion

        #region Jump
        private void HandleJump()
        {
            if (Input.GetButtonDown("Jump") && _isGrounded && !_isCrouching)
            {
                _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
            }
        }
        #endregion

        #region Crouch
        private void HandleCrouch()
        {
            if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.C))
            {
                _isCrouching = !_isCrouching;
            }

            float targetHeight = _isCrouching ? _crouchHeight : _standingHeight;
            _controller.height = Mathf.Lerp(_controller.height, targetHeight, _crouchTransitionSpeed * Time.deltaTime);

            Vector3 center = _controller.center;
            center.y = _controller.height / 2f;
            _controller.center = center;
        }
        #endregion

        #region Gravity
        private void ApplyGravity()
        {
            _velocity.y += _gravity * Time.deltaTime;
            _controller.Move(_velocity * Time.deltaTime);
        }
        #endregion
    }
}