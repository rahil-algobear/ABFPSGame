using UnityEngine;

namespace Game.Weapons
{
    /// <summary>
    /// Base class for all weapons in the game.
    /// </summary>
    public abstract class WeaponBase : MonoBehaviour
    {
        #region Weapon Data
        [Header("Weapon Data")]
        [SerializeField] protected WeaponData _weaponData;

        public WeaponData WeaponData => _weaponData;
        public string WeaponName => _weaponData != null ? _weaponData.weaponName : "Unknown";
        #endregion

        #region Ammo
        [Header("Ammo")]
        [SerializeField] protected int _currentAmmo;
        [SerializeField] protected int _reserveAmmo;

        public int CurrentAmmo => _currentAmmo;
        public int ReserveAmmo => _reserveAmmo;
        public int MaxAmmo => _weaponData != null ? _weaponData.maxAmmo : 0;
        public int MaxReserveAmmo => _weaponData != null ? _weaponData.maxReserveAmmo : 0;
        #endregion

        #region Fire State
        protected bool _isFiring;
        protected float _nextFireTime;

        public bool CanFire => Time.time >= _nextFireTime && _currentAmmo > 0;
        #endregion

        #region Components
        [Header("Components")]
        [SerializeField] protected Transform _firePoint;
        [SerializeField] protected ParticleSystem _muzzleFlash;
        [SerializeField] protected AudioSource _audioSource;
        #endregion

        #region Unity Lifecycle
        protected virtual void Awake()
        {
            if (_weaponData != null)
            {
                _currentAmmo = _weaponData.maxAmmo;
                _reserveAmmo = _weaponData.maxReserveAmmo;
            }

            if (_audioSource == null)
            {
                _audioSource = GetComponent<AudioSource>();
            }
        }

        protected virtual void Start()
        {
            // Override in derived classes
        }

        protected virtual void Update()
        {
            // Override in derived classes
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get current ammo count.
        /// </summary>
        public int GetCurrentAmmo()
        {
            return _currentAmmo;
        }

        /// <summary>
        /// Get reserve ammo count.
        /// </summary>
        public int GetReserveAmmo()
        {
            return _reserveAmmo;
        }

        /// <summary>
        /// Fire the weapon.
        /// </summary>
        public virtual void Fire()
        {
            if (!CanFire) return;

            _currentAmmo--;
            _nextFireTime = Time.time + (1f / (_weaponData != null ? _weaponData.fireRate : 1f));

            // Play muzzle flash
            if (_muzzleFlash != null)
            {
                _muzzleFlash.Play();
            }

            // Play fire sound
            if (_audioSource != null && _weaponData != null && _weaponData.fireSound != null)
            {
                _audioSource.PlayOneShot(_weaponData.fireSound);
            }
        }

        /// <summary>
        /// Reload the weapon.
        /// </summary>
        public virtual void Reload()
        {
            if (_currentAmmo >= MaxAmmo || _reserveAmmo <= 0) return;

            int ammoNeeded = MaxAmmo - _currentAmmo;
            int ammoToReload = Mathf.Min(ammoNeeded, _reserveAmmo);

            _currentAmmo += ammoToReload;
            _reserveAmmo -= ammoToReload;

            // Play reload sound
            if (_audioSource != null && _weaponData != null && _weaponData.reloadSound != null)
            {
                _audioSource.PlayOneShot(_weaponData.reloadSound);
            }
        }

        /// <summary>
        /// Add ammo to reserve.
        /// </summary>
        public virtual void AddAmmo(int amount)
        {
            _reserveAmmo = Mathf.Min(_reserveAmmo + amount, MaxReserveAmmo);
        }

        /// <summary>
        /// Equip the weapon.
        /// </summary>
        public virtual void Equip()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Unequip the weapon.
        /// </summary>
        public virtual void Unequip()
        {
            gameObject.SetActive(false);
        }
        #endregion
    }
}
