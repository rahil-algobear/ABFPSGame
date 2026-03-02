using UnityEngine;
using Game.Core;

namespace Game.Weapons
{
    /// <summary>
    /// Pump-action shotgun that fires multiple pellets.
    /// </summary>
    public class Shotgun : WeaponBase
    {
        [Header("Shotgun Settings")]
        [SerializeField] private int _pelletsPerShot = 8;
        [SerializeField] private float _pelletSpread = 5f;

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
        /// Fires the shotgun (pump-action).
        /// </summary>
        public override void Fire()
        {
            // Check if can fire
            if (_isReloading || Time.time < _nextFireTime || _hasFired)
                return;

            // Check ammo
            if (_currentAmmo <= 0)
            {
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

            // Fire multiple pellets
            for (int i = 0; i < _pelletsPerShot; i++)
            {
                FirePellet();
            }

            // Recoil
            IncreaseSpread();
            ApplyRecoil();
        }

        /// <summary>
        /// Reloads the shotgun.
        /// </summary>
        public override void Reload()
        {
            if (_isReloading || _currentAmmo >= _weaponData.magazineSize || _reserveAmmo <= 0)
                return;

            _isReloading = true;
            
            if (_weaponData.reloadSound != null)
            {
                AudioManager.Instance.PlaySFX(_weaponData.reloadSound);
            }

            Invoke(nameof(FinishReload), _weaponData.reloadTime);
        }

        private void FinishReload()
        {
            int ammoNeeded = _weaponData.magazineSize - _currentAmmo;
            int ammoToReload = Mathf.Min(ammoNeeded, _reserveAmmo);
            
            _currentAmmo += ammoToReload;
            _reserveAmmo -= ammoToReload;
            _isReloading = false;
        }

        /// <summary>
        /// Fires a single shotgun pellet.
        /// </summary>
        private void FirePellet()
        {
            Vector3 direction = _playerCamera.transform.forward;
            
            // Apply shotgun spread
            float spreadX = Random.Range(-_pelletSpread, _pelletSpread);
            float spreadY = Random.Range(-_pelletSpread, _pelletSpread);
            direction = Quaternion.Euler(spreadY, spreadX, 0) * direction;

            if (PerformRaycast(_playerCamera.transform.position, direction, out RaycastHit hit))
            {
                ApplyDamage(hit);
            }
        }

        /// <summary>
        /// Applies camera recoil.
        /// </summary>
        protected override void ApplyRecoil()
        {
            Player.MouseLook mouseLook = GetComponentInParent<Player.MouseLook>();
            if (mouseLook != null)
            {
                float recoil = _weaponData.recoilAmount * 2f; // Shotgun has more recoil
                mouseLook.ApplyRecoil(recoil);
            }
        }
    }
}