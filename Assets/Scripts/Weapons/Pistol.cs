using UnityEngine;
using Game.Core;

namespace Game.Weapons
{
    /// <summary>
    /// Semi-automatic pistol weapon.
    /// </summary>
    public class Pistol : WeaponBase
    {
        private bool _hasFired = false;

        protected override void Update()
        {
            base.Update();

            // Reset fire flag when button is released
            if (!Input.GetButton("Fire1"))
            {
                _hasFired = false;
            }
        }

        /// <summary>
        /// Fires the pistol (semi-automatic).
        /// </summary>
        public override void Fire()
        {
            // Check if can fire
            if (_isReloading || Time.time < _nextFireTime || _hasFired)
                return;

            // Check ammo
            if (_currentAmmo <= 0)
            {
                // Play empty sound
                if (_weaponData.emptySound != null)
                {
                    AudioManager.Instance.PlaySFX(_weaponData.emptySound);
                }
                Reload();
                return;
            }

            // Fire weapon
            _hasFired = true;
            _currentAmmo--;
            _nextFireTime = Time.time + _weaponData.fireRate;

            // Effects
            PlayMuzzleFlash();
            PlayFireSound();
            EjectShell();

            // Raycast
            Vector3 direction = _playerCamera.transform.forward;
            direction = ApplySpread(direction);

            if (PerformRaycast(_playerCamera.transform.position, direction, out RaycastHit hit))
            {
                ApplyDamage(hit);
            }

            // Recoil
            IncreaseSpread();
            ApplyRecoil();
        }

        /// <summary>
        /// Applies camera recoil.
        /// </summary>
        private void ApplyRecoil()
        {
            // Get mouse look component and apply recoil
            Player.MouseLook mouseLook = GetComponentInParent<Player.MouseLook>();
            if (mouseLook != null)
            {
                float recoil = _weaponData.recoilAmount + Random.Range(-_weaponData.recoilVariance, _weaponData.recoilVariance);
                mouseLook.ApplyRecoil(recoil);
            }
        }
    }
}