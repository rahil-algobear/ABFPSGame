using UnityEngine;
using System.Collections;
using Game.Core;

namespace Game.Weapons
{
    /// <summary>
    /// Raycast-based weapon implementation (pistol, rifle, etc.).
    /// </summary>
    public class RaycastWeapon : WeaponBase
    {
        #region Fire Method
        /// <summary>
        /// Fire the weapon.
        /// </summary>
        public override void Fire()
        {
            if (!CanFire()) return;

            // Fire each pellet (for shotguns)
            for (int i = 0; i < _weaponData.pelletsPerShot; i++)
            {
                Vector2 spread = GetSpreadOffset();
                PerformRaycast(spread);
            }

            // Consume ammo
            _currentAmmo--;

            // Effects
            SpawnMuzzleFlash();
            PlayFireSound();
            ApplyRecoil();
            IncreaseSpread();

            // Set next fire time
            _nextFireTime = Time.time + _weaponData.fireRate;

            // Auto reload if empty
            if (_currentAmmo <= 0)
            {
                Reload();
            }
        }
        #endregion

        #region Reload Method
        /// <summary>
        /// Reload the weapon.
        /// </summary>
        public override void Reload()
        {
            if (_isReloading || _currentAmmo >= _weaponData.magazineSize || _reserveAmmo <= 0)
            {
                return;
            }

            StartCoroutine(ReloadCoroutine());
        }

        /// <summary>
        /// Reload coroutine.
        /// </summary>
        private IEnumerator ReloadCoroutine()
        {
            _isReloading = true;

            // Play reload sound
            if (_weaponData.reloadSound != null)
            {
                AudioManager.Instance.PlaySpatialSFX(_weaponData.reloadSound, transform.position);
            }

            yield return new WaitForSeconds(_weaponData.reloadTime);

            // Calculate ammo to reload
            int ammoNeeded = _weaponData.magazineSize - _currentAmmo;
            int ammoToReload = Mathf.Min(ammoNeeded, _reserveAmmo);

            _currentAmmo += ammoToReload;
            _reserveAmmo -= ammoToReload;

            _isReloading = false;
        }
        #endregion
    }
}
