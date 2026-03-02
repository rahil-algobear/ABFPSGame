using UnityEngine;
using Game.Core;

namespace Game.Weapons
{
    /// <summary>
    /// Base class for all weapons in the game.
    /// </summary>
    public abstract class WeaponBase : MonoBehaviour
    {
        #region Weapon Data
        [Header("Weapon Configuration")]
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
        public int MagazineSize => _weaponData != null ? _weaponData.magazineSize : 0;
        #endregion

        #region Fire State
        [Header("Fire State")]
        [SerializeField] protected bool _canFire = true;
        [SerializeField] protected float _nextFireTime = 0f;
        [SerializeField] protected bool _isReloading = false;

        public bool CanFire => _canFire && !_isReloading && _currentAmmo > 0;
        public bool IsReloading => _isReloading;
        #endregion

        #region Components
        [Header("Components")]
        [SerializeField] protected Transform _firePoint;
        [SerializeField] protected ParticleSystem _muzzleFlash;
        [SerializeField] protected AudioSource _audioSource;

        protected Camera _mainCamera;
        #endregion

        #region Unity Lifecycle
        protected virtual void Awake()
        {
            _mainCamera = Camera.main;

            if (_audioSource == null)
            {
                _audioSource = GetComponent<AudioSource>();
            }

            InitializeWeapon();
        }

        protected virtual void Start()
        {
            // Override in subclasses if needed
        }

        protected virtual void Update()
        {
            // Override in subclasses if needed
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initialize weapon with default values.
        /// </summary>
        protected virtual void InitializeWeapon()
        {
            if (_weaponData != null)
            {
                _currentAmmo = _weaponData.magazineSize;
                _reserveAmmo = _weaponData.maxAmmo - _weaponData.magazineSize;
            }
        }
        #endregion

        #region Abstract Methods
        /// <summary>
        /// Fire the weapon. Must be implemented by subclasses.
        /// </summary>
        public abstract void Fire();
        #endregion

        #region Virtual Methods
        /// <summary>
        /// Apply recoil to the weapon. Can be overridden by subclasses.
        /// </summary>
        protected virtual void ApplyRecoil()
        {
            // Default implementation - can be overridden by subclasses
            if (_weaponData != null && _mainCamera != null)
            {
                // Basic recoil implementation
                float recoilAmount = _weaponData.recoil;
                // Subclasses can override this for custom recoil behavior
            }
        }

        /// <summary>
        /// Reload the weapon.
        /// </summary>
        public virtual void Reload()
        {
            if (_isReloading || _currentAmmo == _weaponData.magazineSize || _reserveAmmo <= 0)
                return;

            StartCoroutine(ReloadCoroutine());
        }

        /// <summary>
        /// Reload coroutine.
        /// </summary>
        protected virtual System.Collections.IEnumerator ReloadCoroutine()
        {
            _isReloading = true;
            _canFire = false;

            // Play reload sound
            if (_weaponData != null && _weaponData.reloadSound != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(_weaponData.reloadSound);
            }

            yield return new WaitForSeconds(_weaponData != null ? _weaponData.reloadTime : 1f);

            // Calculate ammo to reload
            int ammoNeeded = _weaponData.magazineSize - _currentAmmo;
            int ammoToReload = Mathf.Min(ammoNeeded, _reserveAmmo);

            _currentAmmo += ammoToReload;
            _reserveAmmo -= ammoToReload;

            _isReloading = false;
            _canFire = true;
        }

        /// <summary>
        /// Add ammo to reserve.
        /// </summary>
        public virtual void AddAmmo(int amount)
        {
            _reserveAmmo = Mathf.Min(_reserveAmmo + amount, _weaponData.maxAmmo - _weaponData.magazineSize);
        }

        /// <summary>
        /// Check if weapon can fire based on fire rate.
        /// </summary>
        protected virtual bool CheckFireRate()
        {
            if (Time.time >= _nextFireTime)
            {
                _nextFireTime = Time.time + (1f / _weaponData.fireRate);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Play muzzle flash effect.
        /// </summary>
        protected virtual void PlayMuzzleFlash()
        {
            if (_muzzleFlash != null)
            {
                _muzzleFlash.Play();
            }
        }

        /// <summary>
        /// Play fire sound.
        /// </summary>
        protected virtual void PlayFireSound()
        {
            if (_weaponData != null && _weaponData.fireSound != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(_weaponData.fireSound);
            }
        }

        /// <summary>
        /// Equip the weapon.
        /// </summary>
        public virtual void Equip()
        {
            gameObject.SetActive(true);
            _canFire = true;
        }

        /// <summary>
        /// Unequip the weapon.
        /// </summary>
        public virtual void Unequip()
        {
            gameObject.SetActive(false);
            _canFire = false;
        }
        #endregion
    }
}