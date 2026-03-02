using UnityEngine;
using Game.Core;
using Game.Player;

namespace Game.Weapons
{
    /// <summary>
    /// Base class for all weapons with common functionality.
    /// </summary>
    public abstract class WeaponBase : MonoBehaviour
    {
        #region Weapon Data
        [Header("Weapon Data")]
        [SerializeField] protected WeaponData _weaponData;

        protected int _currentAmmo;
        protected int _reserveAmmo;
        protected float _nextFireTime;
        protected bool _isReloading;
        protected bool _isAiming;
        #endregion

        #region Components
        protected MouseLook _mouseLook;
        #endregion

        #region Properties
        public WeaponData WeaponData => _weaponData;
        public bool CanFire => !_isReloading && _currentAmmo > 0 && Time.time >= _nextFireTime;
        public bool IsReloading => _isReloading;
        public bool IsAiming => _isAiming;
        #endregion

        #region Unity Lifecycle
        protected virtual void Awake()
        {
            if (_weaponData != null)
            {
                _currentAmmo = _weaponData.MagazineSize;
                _reserveAmmo = _weaponData.MaxReserveAmmo;
            }

            _mouseLook = FindObjectOfType<MouseLook>();
        }

        protected virtual void Start()
        {
            // Override in subclasses
        }

        protected virtual void Update()
        {
            // Override in subclasses
        }
        #endregion

        #region Abstract Methods
        public abstract void Fire();
        #endregion

        #region Virtual Methods
        protected virtual void ApplyRecoil()
        {
            if (_mouseLook != null && _weaponData != null)
            {
                _mouseLook.ApplyRecoil(_weaponData.RecoilAmount, _weaponData.RecoilAmount * 0.5f);
            }
        }

        public virtual void Reload()
        {
            if (_isReloading || _currentAmmo == _weaponData.MagazineSize || _reserveAmmo <= 0)
                return;

            StartCoroutine(ReloadCoroutine());
        }

        protected virtual System.Collections.IEnumerator ReloadCoroutine()
        {
            _isReloading = true;

            yield return new WaitForSeconds(_weaponData.ReloadTime);

            int ammoNeeded = _weaponData.MagazineSize - _currentAmmo;
            int ammoToReload = Mathf.Min(ammoNeeded, _reserveAmmo);

            _currentAmmo += ammoToReload;
            _reserveAmmo -= ammoToReload;

            _isReloading = false;
        }

        public virtual void AimDownSights(bool aiming)
        {
            _isAiming = aiming;
        }

        protected virtual Vector3 GetSpreadOffset()
        {
            if (_weaponData == null) return Vector3.zero;

            float spread = _isAiming ? _weaponData.SpreadAmount * 0.5f : _weaponData.SpreadAmount;
            return new Vector3(
                Random.Range(-spread, spread),
                Random.Range(-spread, spread),
                0f
            );
        }

        protected virtual bool PerformRaycast(Vector3 origin, Vector3 direction, out RaycastHit hit)
        {
            return Physics.Raycast(origin, direction, out hit, _weaponData.Range);
        }

        protected virtual void SpawnMuzzleFlash()
        {
            // Override in subclasses if needed
        }
        #endregion

        #region Public Methods
        public int GetCurrentAmmo()
        {
            return _currentAmmo;
        }

        public int GetReserveAmmo()
        {
            return _reserveAmmo;
        }

        public void AddReserveAmmo(int amount)
        {
            _reserveAmmo = Mathf.Min(_reserveAmmo + amount, _weaponData.MaxReserveAmmo);
        }
        #endregion
    }
}