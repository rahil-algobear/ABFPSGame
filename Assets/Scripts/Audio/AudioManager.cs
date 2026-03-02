using UnityEngine;
using System.Collections.Generic;

namespace Game.Audio
{
    /// <summary>
    /// Centralized audio management system for music and sound effects.
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
        [SerializeField] private int _sfxPoolSize = 10;
        #endregion

        #region Volume Settings
        [Header("Volume")]
        [SerializeField] private float _masterVolume = 1f;
        [SerializeField] private float _musicVolume = 0.5f;
        [SerializeField] private float _sfxVolume = 1f;
        #endregion

        #region Object Pool
        private List<AudioSource> _sfxPool = new List<AudioSource>();
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
        /// Initialize audio sources and pool.
        /// </summary>
        private void InitializeAudioSources()
        {
            // Create music source if not assigned
            if (_musicSource == null)
            {
                GameObject musicObj = new GameObject("MusicSource");
                musicObj.transform.parent = transform;
                _musicSource = musicObj.AddComponent<AudioSource>();
                _musicSource.loop = true;
                _musicSource.playOnAwake = false;
            }

            // Create SFX source if not assigned
            if (_sfxSource == null)
            {
                GameObject sfxObj = new GameObject("SFXSource");
                sfxObj.transform.parent = transform;
                _sfxSource = sfxObj.AddComponent<AudioSource>();
                _sfxSource.playOnAwake = false;
            }

            // Create SFX pool
            for (int i = 0; i < _sfxPoolSize; i++)
            {
                GameObject poolObj = new GameObject($"SFXPool_{i}");
                poolObj.transform.parent = transform;
                AudioSource source = poolObj.AddComponent<AudioSource>();
                source.playOnAwake = false;
                _sfxPool.Add(source);
            }

            UpdateVolumes();
        }
        #endregion

        #region Music
        /// <summary>
        /// Play background music.
        /// </summary>
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

        #region Sound Effects
        /// <summary>
        /// Play one-shot sound effect.
        /// </summary>
        public void PlaySFX(AudioClip clip, float volumeScale = 1f)
        {
            if (_sfxSource == null || clip == null) return;

            _sfxSource.PlayOneShot(clip, volumeScale * _sfxVolume * _masterVolume);
        }

        /// <summary>
        /// Play 3D spatial sound effect.
        /// </summary>
        public void PlaySFX3D(AudioClip clip, Vector3 position, float volumeScale = 1f)
        {
            if (clip == null) return;

            AudioSource source = GetAvailableSFXSource();
            if (source != null)
            {
                source.transform.position = position;
                source.clip = clip;
                source.volume = volumeScale * _sfxVolume * _masterVolume;
                source.spatialBlend = 1f; // 3D sound
                source.Play();
            }
        }

        /// <summary>
        /// Get available audio source from pool.
        /// </summary>
        private AudioSource GetAvailableSFXSource()
        {
            foreach (AudioSource source in _sfxPool)
            {
                if (!source.isPlaying)
                {
                    return source;
                }
            }
            return _sfxPool[0]; // Return first if all busy
        }
        #endregion

        #region Volume Control
        /// <summary>
        /// Set master volume.
        /// </summary>
        public void SetMasterVolume(float volume)
        {
            _masterVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }

        /// <summary>
        /// Set music volume.
        /// </summary>
        public void SetMusicVolume(float volume)
        {
            _musicVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }

        /// <summary>
        /// Set SFX volume.
        /// </summary>
        public void SetSFXVolume(float volume)
        {
            _sfxVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }

        /// <summary>
        /// Update all audio source volumes.
        /// </summary>
        private void UpdateVolumes()
        {
            if (_musicSource != null)
            {
                _musicSource.volume = _musicVolume * _masterVolume;
            }

            if (_sfxSource != null)
            {
                _sfxSource.volume = _sfxVolume * _masterVolume;
            }
        }
        #endregion
    }
}
