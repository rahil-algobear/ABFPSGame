using UnityEngine;
using Game.Core;
using Game.Player;

namespace Game.Weapons
{
    /// <summary>
    /// Abstract base class for all weapons.
    /// </summary>
    public abstract class WeaponBase : MonoBehaviour
    {
        #region Weapon Data
        [Header("Weapon Configuration")]
        [SerializeField] protected WeaponData _weaponData;

        public WeaponData WeaponData => _weaponData;
        #endregion

        #region Ammo
        protected int _currentAmmo;
        protected int _reserveAmmo;

        public int CurrentAmmo => _currentAmmo;
        public int ReserveAmmo => _reserveAmmo;
        public int MagazineSize => _weaponData.magazineSize;
        #endregion

        #region Fire State
        protected float _nextFireTime;
        protected float _currentSpread;
        protected bool _isReloading;
        protected bool _isAiming;
        #endregion

        #region Components
        [Header("References")]
        [SerializeField] protected Transform _firePoint;
        [SerializeField] protected Camera _playerCamera;
        [SerializeField] protected MouseLook _mouseLook;

        protected LayerMask _hitMask;
        #endregion

        #region Unity Lifecycle
        protected virtual void Awake()
        {
            if (_playerCamera == null)
            {
                _playerCamera = Camera.main;
            }

            if (_mouseLook == null)
            {
                _mouseLook = FindObjectOfType<MouseLook>();
            }

            // Everything except player layer
            _hitMask = ~(1 << LayerMask.NameToLayer("Player"));
        }

        protected virtual void Start()
        {
            _currentAmmo = _weaponData.magazineSize;
            _reserveAmmo = _weaponData.maxReserveAmmo;
            _currentSpread = _weaponData.baseSpread;
        }

        protected virtual void Update()
        {
            if (GameManager.Instance.IsPaused || GameManager.Instance.IsGameOver)
            {
                return;
            }

            HandleSpreadRecovery();
        }
        #endregion

        #region Abstract Methods
        /// <summary>
        /// Fire the weapon.
        /// </summary>
        public abstract void Fire();

        /// <summary>
        /// Reload the weapon.
        /// </summary>
        public abstract void Reload();
        #endregion

        #region Virtual Methods
        /// <summary>
        /// Aim down sights.
        /// </summary>
        /// <param name="aiming">True if aiming</param>
        public virtual void AimDownSights(bool aiming)
        {
            _isAiming = aiming;
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

        #region Protected Methods
        /// <summary>
        /// Check if weapon can fire.
        /// </summary>
        /// <returns>True if can fire</returns>
        protected virtual bool CanFire()
        {
            return !_isReloading && Time.time >= _nextFireTime && _currentAmmo > 0;
        }

        /// <summary>
        /// Perform raycast hit detection.
        /// </summary>
        /// <param name="spreadOffset">Spread offset for this shot</param>
        protected virtual void PerformRaycast(Vector2 spreadOffset)
        {
            if (_playerCamera == null || _firePoint == null) return;

            // Calculate ray direction with spread
            Vector3 direction = _playerCamera.transform.forward;
            direction += _playerCamera.transform.right * spreadOffset.x;
            direction += _playerCamera.transform.up * spreadOffset.y;
            direction.Normalize();

            Ray ray = new Ray(_firePoint.position, direction);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, _weaponData.maxRange, _hitMask))
            {
                ProcessHit(hit);
            }
        }

        /// <summary>
        /// Process a successful hit.
        /// </summary>
        /// <param name="hit">RaycastHit data</param>
        protected virtual void ProcessHit(RaycastHit hit)
        {
            float distance = hit.distance;
            float damage = _weaponData.GetDamageAtDistance(distance);

            // Check for headshot
            if (hit.collider.CompareTag("EnemyHead"))
            {
                damage *= _weaponData.headshotMultiplier;
            }

            // Apply damage to hit object
            var damageable = hit.collider.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage, hit.point, -hit.normal);
            }

            // Spawn impact effect
            if (_weaponData.bulletImpactPrefab != null)
            {
                GameObject impact = Instantiate(
                    _weaponData.bulletImpactPrefab,
                    hit.point,
                    Quaternion.LookRotation(hit.normal)
                );
                Destroy(impact, 2f);
            }
        }

        /// <summary>
        /// Apply recoil to camera.
        /// </summary>
        protected virtual void ApplyRecoil()
        {
            if (_mouseLook == null) return;

            Vector2 recoil = new Vector2(
                Random.Range(_weaponData.recoilMin.x, _weaponData.recoilMax.x),
                Random.Range(_weaponData.recoilMin.y, _weaponData.recoilMax.y)
            );

            _mouseLook.AddRecoil(recoil);
        }

        /// <summary>
        /// Increase weapon spread.
        /// </summary>
        protected virtual void IncreaseSpread()
        {
            _currentSpread += _weaponData.spreadIncreasePerShot;
            _currentSpread = Mathf.Min(_currentSpread, _weaponData.maxSpread);
        }

        /// <summary>
        /// Handle spread recovery over time.
        /// </summary>
        protected virtual void HandleSpreadRecovery()
        {
            if (_currentSpread > _weaponData.baseSpread)
            {
                _currentSpread -= _weaponData.spreadDecreaseRate * Time.deltaTime;
                _currentSpread = Mathf.Max(_currentSpread, _weaponData.baseSpread);
            }
        }

        /// <summary>
        /// Get random spread offset.
        /// </summary>
        /// <returns>Spread offset vector</returns>
        protected virtual Vector2 GetSpreadOffset()
        {
            float spread = _isAiming ? _currentSpread * 0.5f : _currentSpread;
            return Random.insideUnitCircle * spread * 0.01f;
        }

        /// <summary>
        /// Spawn muzzle flash effect.
        /// </summary>
        protected virtual void SpawnMuzzleFlash()
        {
            if (_weaponData.muzzleFlashPrefab != null && _firePoint != null)
            {
                GameObject flash = Instantiate(
                    _weaponData.muzzleFlashPrefab,
                    _firePoint.position,
                    _firePoint.rotation,
                    _firePoint
                );
                Destroy(flash, 0.1f);
            }
        }

        /// <summary>
        /// Play fire sound.
        /// </summary>
        protected virtual void PlayFireSound()
        {
            if (_weaponData.fireSound != null)
            {
                AudioManager.Instance.PlaySpatialSFX(_weaponData.fireSound, transform.position);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Add ammo to reserve.
        /// </summary>
        /// <param name="amount">Amount to add</param>
        public void AddAmmo(int amount)
        {
            _reserveAmmo += amount;
            _reserveAmmo = Mathf.Min(_reserveAmmo, _weaponData.maxReserveAmmo);
        }

        /// <summary>
        /// Check if weapon is reloading.
        /// </summary>
        /// <returns>True if reloading</returns>
        public bool IsReloading()
        {
            return _isReloading;
        }

        /// <summary>
        /// Check if weapon is aiming.
        /// </summary>
        /// <returns>True if aiming</returns>
        public bool IsAiming()
        {
            return _isAiming;
        }
        #endregion
    }

    /// <summary>
    /// Interface for objects that can take damage.
    /// </summary>
    public interface IDamageable
    {
        void TakeDamage(float damage, Vector3 hitPoint, Vector3 hitDirection);
    }
}
