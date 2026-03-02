using UnityEngine;

namespace Game.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float _walkSpeed = 5f;
        [SerializeField] private float _sprintSpeed = 8f;
        [SerializeField] private float _crouchSpeed = 2.5f;
        [SerializeField] private float _jumpHeight = 2f;
        [SerializeField] private float _gravity = -20f;

        [Header("Ground Check")]
        [SerializeField] private Transform _groundCheck;
        [SerializeField] private float _groundDistance = 0.4f;
        [SerializeField] private LayerMask _groundMask;

        private CharacterController _controller;
        private Vector3 _velocity;
        private bool _isGrounded;
        private bool _isSprinting;
        private bool _isCrouching;

        public bool IsMoving => _controller.velocity.magnitude > 0.1f;
        public bool IsSprinting => _isSprinting;
        public bool IsCrouching => _isCrouching;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            CheckGround();
            HandleMovement();
            HandleJump();
            ApplyGravity();
        }

        private void CheckGround()
        {
            _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundDistance, _groundMask);

            if (_isGrounded && _velocity.y < 0)
            {
                _velocity.y = -2f;
            }
        }

        private void HandleMovement()
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;

            _isSprinting = Input.GetKey(KeyCode.LeftShift) && z > 0 && !_isCrouching;
            _isCrouching = Input.GetKey(KeyCode.LeftControl);

            float speed = _isSprinting ? _sprintSpeed : (_isCrouching ? _crouchSpeed : _walkSpeed);

            _controller.Move(move * speed * Time.deltaTime);
        }

        private void HandleJump()
        {
            if (Input.GetButtonDown("Jump") && _isGrounded && !_isCrouching)
            {
                _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
            }
        }

        private void ApplyGravity()
        {
            _velocity.y += _gravity * Time.deltaTime;
            _controller.Move(_velocity * Time.deltaTime);
        }
    }
}