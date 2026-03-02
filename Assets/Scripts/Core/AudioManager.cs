using UnityEngine;
using System.Collections.Generic;

namespace Game.Core
{
    /// <summary>
    /// Singleton AudioManager for centralized audio playback.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        #region Singleton
        private static AudioManager _instance;
        public static AudioManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<AudioManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("AudioManager");
                        _instance = go.AddComponent<AudioManager>();
                    }
                }
                return _instance;
            }
        }
        #endregion

        #region Audio Sources
        [Header("Audio Sources")]
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _sfxSource;
        [SerializeField] private int _maxSpatialSources = 10;

        private List<AudioSource> _spatialSourcePool = new List<AudioSource>();
        #endregion

        #region Volume Settings
        [Header("Volume Settings")]
        [SerializeField] [Range(0f, 1f)] private float _masterVolume = 1f;
        [SerializeField] [Range(0f, 1f)] private float _musicVolume = 0.6f;
        [SerializeField] [Range(0f, 1f)] private float _sfxVolume = 0.8f;

        public float MasterVolume
        {
            get => _masterVolume;
            set
            {
                _masterVolume = Mathf.Clamp01(value);
                UpdateVolumes();
            }
        }

        public float MusicVolume
        {
            get => _musicVolume;
            set
            {
                _musicVolume = Mathf.Clamp01(value);
                UpdateVolumes();
            }
        }

        public float SFXVolume
        {
            get => _sfxVolume;
            set
            {
                _sfxVolume = Mathf.Clamp01(value);
                UpdateVolumes();
            }
        }
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
            DontDestroyOnLoad(gameObject);

            InitializeAudioSources();
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initialize audio sources.
        /// </summary>
        private void InitializeAudioSources()
        {
            // Create music source if not assigned
            if (_musicSource == null)
            {
                _musicSource = gameObject.AddComponent<AudioSource>();
                _musicSource.loop = true;
                _musicSource.playOnAwake = false;
            }

            // Create SFX source if not assigned
            if (_sfxSource == null)
            {
                _sfxSource = gameObject.AddComponent<AudioSource>();
                _sfxSource.playOnAwake = false;
            }

            // Create spatial audio source pool
            for (int i = 0; i < _maxSpatialSources; i++)
            {
                AudioSource source = gameObject.AddComponent<AudioSource>();
                source.playOnAwake = false;
                source.spatialBlend = 1f; // Full 3D
                source.minDistance = 1f;
                source.maxDistance = 50f;
                source.rolloffMode = AudioRolloffMode.Linear;
                _spatialSourcePool.Add(source);
            }

            UpdateVolumes();
        }

        /// <summary>
        /// Update all audio source volumes.
        /// </summary>
        private void UpdateVolumes()
        {
            if (_musicSource != null)
            {
                _musicSource.volume = _masterVolume * _musicVolume;
            }

            if (_sfxSource != null)
            {
                _sfxSource.volume = _masterVolume * _sfxVolume;
            }

            foreach (var source in _spatialSourcePool)
            {
                if (source != null)
                {
                    source.volume = _masterVolume * _sfxVolume;
                }
            }
        }
        #endregion

        #region Public Methods - Music
        /// <summary>
        /// Play background music.
        /// </summary>
        /// <param name="clip">Audio clip to play</param>
        /// <param name="loop">Should the music loop</param>
        public void PlayMusic(AudioClip clip, bool loop = true)
        {
            if (_musicSource == null || clip == null) return;

            _musicSource.clip = clip;
            _musicSource.loop = loop;
            _musicSource.Play();
        }

        /// <summary>
        /// Stop background music.
        /// </summary>
        public void StopMusic()
        {
            if (_musicSource != null)
            {
                _musicSource.Stop();
            }
        }

        /// <summary>
        /// Pause background music.
        /// </summary>
        public void PauseMusic()
        {
            if (_musicSource != null)
            {
                _musicSource.Pause();
            }
        }

        /// <summary>
        /// Resume background music.
        /// </summary>
        public void ResumeMusic()
        {
            if (_musicSource != null)
            {
                _musicSource.UnPause();
            }
        }
        #endregion

        #region Public Methods - SFX
        /// <summary>
        /// Play a one-shot sound effect.
        /// </summary>
        /// <param name="clip">Audio clip to play</param>
        /// <param name="volumeScale">Volume scale multiplier</param>
        public void PlaySFX(AudioClip clip, float volumeScale = 1f)
        {
            if (_sfxSource == null || clip == null) return;

            _sfxSource.PlayOneShot(clip, volumeScale);
        }

        /// <summary>
        /// Play a spatial 3D sound effect at a position.
        /// </summary>
        /// <param name="clip">Audio clip to play</param>
        /// <param name="position">World position</param>
        /// <param name="volumeScale">Volume scale multiplier</param>
        public void PlaySFX3D(AudioClip clip, Vector3 position, float volumeScale = 1f)
        {
            if (clip == null) return;

            AudioSource availableSource = GetAvailableSpatialSource();
            if (availableSource != null)
            {
                availableSource.transform.position = position;
                availableSource.PlayOneShot(clip, volumeScale);
            }
        }

        /// <summary>
        /// Play a spatial 3D sound effect attached to a transform.
        /// </summary>
        /// <param name="clip">Audio clip to play</param>
        /// <param name="parent">Transform to attach to</param>
        /// <param name="volumeScale">Volume scale multiplier</param>
        public void PlaySpatialSFX(AudioClip clip, Vector3 position, float volumeScale = 1f)
        {
            PlaySFX3D(clip, position, volumeScale);
        }

        /// <summary>
        /// Play a spatial 3D sound effect attached to a transform.
        /// </summary>
        /// <param name="clip">Audio clip to play</param>
        /// <param name="parent">Transform to attach to</param>
        /// <param name="volumeScale">Volume scale multiplier</param>
        public void PlaySpatialSFX(AudioClip clip, Transform parent, float volumeScale = 1f)
        {
            if (clip == null || parent == null) return;

            AudioSource availableSource = GetAvailableSpatialSource();
            if (availableSource != null)
            {
                availableSource.transform.position = parent.position;
                availableSource.PlayOneShot(clip, volumeScale);
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Get an available spatial audio source from the pool.
        /// </summary>
        /// <returns>Available AudioSource or null</returns>
        private AudioSource GetAvailableSpatialSource()
        {
            foreach (var source in _spatialSourcePool)
            {
                if (!source.isPlaying)
                {
                    return source;
                }
            }

            // All sources busy, return the first one (will interrupt)
            return _spatialSourcePool.Count > 0 ? _spatialSourcePool[0] : null;
        }
        #endregion
    }
}
