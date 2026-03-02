using UnityEngine;
using Game.Core;

namespace Game.Player
{
    [RequireComponent(typeof(AudioSource))]
    public class FootstepAudio : MonoBehaviour
    {
        [Header("Footstep Sounds")]
        [SerializeField] private AudioClip[] _footstepSounds;
        [SerializeField] private float _footstepInterval = 0.5f;
        [SerializeField] private float _sprintFootstepInterval = 0.3f;

        [Header("Volume")]
        [SerializeField] private float _walkVolume = 0.5f;
        [SerializeField] private float _sprintVolume = 0.7f;

        private AudioSource _audioSource;
        private PlayerController _playerController;
        private float _lastFootstepTime;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _playerController = GetComponent<PlayerController>();
        }

        private void Update()
        {
            if (_playerController == null || _footstepSounds.Length == 0)
                return;

            bool isMoving = _playerController.IsMoving;
            bool isSprinting = _playerController.IsSprinting;

            if (isMoving)
            {
                float interval = isSprinting ? _sprintFootstepInterval : _footstepInterval;
                float volume = isSprinting ? _sprintVolume : _walkVolume;

                if (Time.time >= _lastFootstepTime + interval)
                {
                    PlayFootstep(volume);
                    _lastFootstepTime = Time.time;
                }
            }
        }

        private void PlayFootstep(float volume)
        {
            if (_footstepSounds.Length == 0) return;

            AudioClip clip = _footstepSounds[Random.Range(0, _footstepSounds.Length)];
            _audioSource.PlayOneShot(clip, volume);
        }
    }
}