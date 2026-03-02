using UnityEngine;
using System.Collections;
using Game.Core;

namespace Game.Level
{
    /// <summary>
    /// Controls automatic door opening and closing.
    /// </summary>
    public class DoorController : MonoBehaviour
    {
        #region Door Settings
        [Header("Door Settings")]
        [SerializeField] private Transform _doorTransform;
        [SerializeField] private Vector3 _openPosition;
        [SerializeField] private Vector3 _closedPosition;
        [SerializeField] private float _openSpeed = 2f;
        [SerializeField] private float _closeDelay = 2f;
        [SerializeField] private bool _requiresKey = false;
        #endregion

        #region Trigger Settings
        [Header("Trigger Settings")]
        [SerializeField] private float _triggerRange = 3f;
        [SerializeField] private LayerMask _playerMask;
        #endregion

        #region Audio
        [Header("Audio")]
        [SerializeField] private AudioClip _openSound;
        [SerializeField] private AudioClip _closeSound;
        [SerializeField] private AudioClip _lockedSound;
        #endregion

        #region State
        private bool _isOpen = false;
        private bool _isMoving = false;
        private bool _isLocked = false;
        private Transform _player;
        private Coroutine _closeCoroutine;
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            if (_doorTransform == null)
            {
                _doorTransform = transform;
            }

            _closedPosition = _doorTransform.localPosition;
            _openPosition = _closedPosition + Vector3.up * 3f;

            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                _player = playerObj.transform;
            }

            _isLocked = _requiresKey;
        }

        private void Update()
        {
            CheckPlayerProximity();
        }
        #endregion

        #region Door Control
        /// <summary>
        /// Checks if the player is near the door.
        /// </summary>
        private void CheckPlayerProximity()
        {
            if (_player == null || _isMoving)
                return;

            float distance = Vector3.Distance(transform.position, _player.position);

            if (distance <= _triggerRange && !_isOpen)
            {
                if (_isLocked)
                {
                    PlayLockedSound();
                }
                else
                {
                    OpenDoor();
                }
            }
            else if (distance > _triggerRange && _isOpen)
            {
                if (_closeCoroutine == null)
                {
                    _closeCoroutine = StartCoroutine(CloseAfterDelay());
                }
            }
        }

        /// <summary>
        /// Opens the door.
        /// </summary>
        public void OpenDoor()
        {
            if (_isOpen || _isMoving || _isLocked)
                return;

            if (_closeCoroutine != null)
            {
                StopCoroutine(_closeCoroutine);
                _closeCoroutine = null;
            }

            StartCoroutine(MoveDoor(_openPosition, true));

            if (_openSound != null)
            {
                AudioManager.Instance.PlaySFX3D(_openSound, transform.position);
            }
        }

        /// <summary>
        /// Closes the door.
        /// </summary>
        public void CloseDoor()
        {
            if (!_isOpen || _isMoving)
                return;

            StartCoroutine(MoveDoor(_closedPosition, false));

            if (_closeSound != null)
            {
                AudioManager.Instance.PlaySFX3D(_closeSound, transform.position);
            }
        }

        /// <summary>
        /// Moves the door to a target position.
        /// </summary>
        private IEnumerator MoveDoor(Vector3 targetPosition, bool opening)
        {
            _isMoving = true;

            while (Vector3.Distance(_doorTransform.localPosition, targetPosition) > 0.01f)
            {
                _doorTransform.localPosition = Vector3.Lerp(
                    _doorTransform.localPosition,
                    targetPosition,
                    Time.deltaTime * _openSpeed
                );
                yield return null;
            }

            _doorTransform.localPosition = targetPosition;
            _isOpen = opening;
            _isMoving = false;
        }

        /// <summary>
        /// Closes the door after a delay.
        /// </summary>
        private IEnumerator CloseAfterDelay()
        {
            yield return new WaitForSeconds(_closeDelay);
            CloseDoor();
            _closeCoroutine = null;
        }
        #endregion

        #region Lock Control
        /// <summary>
        /// Unlocks the door.
        /// </summary>
        public void Unlock()
        {
            _isLocked = false;
        }

        /// <summary>
        /// Locks the door.
        /// </summary>
        public void Lock()
        {
            _isLocked = true;
        }

        /// <summary>
        /// Plays the locked sound.
        /// </summary>
        private void PlayLockedSound()
        {
            if (_lockedSound != null)
            {
                AudioManager.Instance.PlaySFX3D(_lockedSound, transform.position);
            }
        }
        #endregion

        #region Gizmos
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _triggerRange);
        }
        #endregion
    }
}