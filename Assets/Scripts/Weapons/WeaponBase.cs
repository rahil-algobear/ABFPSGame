using UnityEngine;
using Game.Core;

namespace Game.Weapons
{
    public abstract class WeaponBase : MonoBehaviour
    {
        [Header("Weapon Data")]
        [SerializeField] protected WeaponData _weaponData;

        [Header("Ammo")]
        protected int _currentAmmo;
        protected int _reserveAmmo;

        [Header("State")]
        protected float _lastFireTime;
        protected bool _isReloading;
        protected bool _isEquipped;

        public WeaponData WeaponData => _weaponData;
        public bool CanFire => _currentAmmo > 0 && !_isReloading && Time.time >= _lastFireTime + (1f / _weaponData.fireRate);
        public bool IsReloading => _isReloading;

        protected virtual void Awake()
        {
            _currentAmmo = _weaponData.magazineSize;
            _reserveAmmo = _weaponData.maxAmmo;
        }

        public abstract void Fire();

        public virtual void Reload()
        {
            if (_isReloading || _currentAmmo == _weaponData.magazineSize || _reserveAmmo <= 0)
                return;

            _isReloading = true;
            Invoke(nameof(CompleteReload), _weaponData.reloadTime);
        }

        protected virtual void CompleteReload()
        {
            int ammoNeeded = _weaponData.magazineSize - _currentAmmo;
            int ammoToReload = Mathf.Min(ammoNeeded, _reserveAmmo);

            _currentAmmo += ammoToReload;
            _reserveAmmo -= ammoToReload;
            _isReloading = false;
        }

        public virtual void AddAmmo(int amount)
        {
            _reserveAmmo = Mathf.Min(_reserveAmmo + amount, _weaponData.maxAmmo);
        }

        public int GetCurrentAmmo() => _currentAmmo;
        public int GetReserveAmmo() => _reserveAmmo;

        public virtual void AddReserveAmmo(int amount)
        {
            _reserveAmmo = Mathf.Min(_reserveAmmo + amount, _weaponData.maxAmmo);
        }

        public virtual void Equip()
        {
            _isEquipped = true;
            gameObject.SetActive(true);
        }

        public virtual void Unequip()
        {
            _isEquipped = false;
            gameObject.SetActive(false);
        }

        public virtual void AimDownSights(bool isAiming)
        {
            // Override in derived classes for ADS behavior
        }

        protected virtual void ApplyRecoil()
        {
            // Override in derived classes for recoil behavior
        }
    }
}