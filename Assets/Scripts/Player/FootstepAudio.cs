using UnityEngine;
using Game.Core;

namespace Game.Player
{
    /// <summary>
    /// Footstep audio system tied to player movement state.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class FootstepAudio : MonoBehaviour
    {
        #region Components
        private AudioSource _audioSource;
        private PlayerController _playerController;
        #endregion

        #region Settings
        [Header("Footstep Settings")]
        [SerializeField] private AudioClip[] _footstepSounds;
        [SerializeField] private float _walkStepInterval = 0.5f;
        [SerializeField] private float _sprintStepInterval = 0.3f;
        [SerializeField] [Range(0f, 1f)] private float _footstepVolume = 0.5f;
        #endregion

        #region State
        private float _stepTimer = 0f;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _playerController = GetComponent<PlayerController>();

            if (_audioSource != null)
            {
                _audioSource.playOnAwake = false;
                _audioSource.spatialBlend = 0f; // 2D sound
            }
        }

        private void Update()
        {
            if (GameManager.Instance.IsPaused || GameManager.Instance.IsGameOver)
                return;

            HandleFootsteps();
        }
        #endregion

        #region Footstep Logic
        /// <summary>
        /// Handle footstep audio based on movement state.
        /// </summary>
        private void HandleFootsteps()
        {
            if (_playerController == null || !_playerController.IsGrounded() || !_playerController.IsMoving())
            {
                _stepTimer = 0f;
                return;
            }

            float stepInterval = _playerController.IsSprinting() ? _sprintStepInterval : _walkStepInterval;
            _stepTimer += Time.deltaTime;

            if (_stepTimer >= stepInterval)
            {
                PlayFootstep();
                _stepTimer = 0f;
            }
        }

        /// <summary>
        /// Play a random footstep sound.
        /// </summary>
        private void PlayFootstep()
        {
            if (_footstepSounds == null || _footstepSounds.Length == 0 || _audioSource == null)
                return;

            AudioClip clip = _footstepSounds[Random.Range(0, _footstepSounds.Length)];
            if (clip != null)
            {
                _audioSource.PlayOneShot(clip, _footstepVolume);
            }
        }
        #endregion
    }
}
