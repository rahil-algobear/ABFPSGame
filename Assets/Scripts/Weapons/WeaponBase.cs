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
        [SerializeField] protected Transform _muzzlePoint;
        [SerializeField] protected Camera _playerCamera;
        [SerializeField] protected MouseLook _mouseLook;
        [SerializeField] protected ParticleSystem _muzzleFlash;
        [SerializeField] protected GameObject _shellEjectPrefab;
        [SerializeField] protected Transform _shellEjectPoint;

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

        #region Spread
        /// <summary>
        /// Apply spread to firing direction.
        /// </summary>
        protected Vector3 ApplySpread(Vector3 direction)
        {
            float spreadX = Random.Range(-_currentSpread, _currentSpread);
            float spreadY = Random.Range(-_currentSpread, _currentSpread);
            return Quaternion.Euler(spreadY, spreadX, 0) * direction;
        }

        /// <summary>
        /// Increase spread after firing.
        /// </summary>
        protected void IncreaseSpread()
        {
            _currentSpread = Mathf.Min(_currentSpread + _weaponData.spreadIncrease, _weaponData.maxSpread);
        }

        /// <summary>
        /// Recover spread over time.
        /// </summary>
        protected void HandleSpreadRecovery()
        {
            if (_currentSpread > _weaponData.baseSpread)
            {
                _currentSpread = Mathf.Max(_currentSpread - _weaponData.spreadRecovery * Time.deltaTime, _weaponData.baseSpread);
            }
        }
        #endregion

        #region Raycast
        /// <summary>
        /// Perform raycast for hitscan weapons.
        /// </summary>
        protected bool PerformRaycast(Vector3 origin, Vector3 direction, out RaycastHit hit)
        {
            return Physics.Raycast(origin, direction, out hit, _weaponData.range, _hitMask);
        }

        /// <summary>
        /// Apply damage to hit target.
        /// </summary>
        protected void ApplyDamage(RaycastHit hit)
        {
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                float distance = hit.distance;
                float finalDamage = DamageSystem.CalculateDamage(
                    _weaponData.damage,
                    DamageSystem.DetermineHitLocation(hit.collider),
                    _weaponData.damageType,
                    distance,
                    _weaponData.range
                );

                damageable.TakeDamage(finalDamage, _weaponData.damageType, hit.point);
            }
        }
        #endregion

        #region Effects
        /// <summary>
        /// Play muzzle flash effect.
        /// </summary>
        protected void PlayMuzzleFlash()
        {
            if (_muzzleFlash != null)
            {
                _muzzleFlash.Play();
            }
        }

        /// <summary>
        /// Play fire sound.
        /// </summary>
        protected void PlayFireSound()
        {
            if (_weaponData.fireSound != null)
            {
                AudioManager.Instance.PlaySFX(_weaponData.fireSound);
            }
        }

        /// <summary>
        /// Eject shell casing.
        /// </summary>
        protected void EjectShell()
        {
            if (_shellEjectPrefab != null && _shellEjectPoint != null)
            {
                GameObject shell = Instantiate(_shellEjectPrefab, _shellEjectPoint.position, _shellEjectPoint.rotation);
                Rigidbody rb = shell.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(_shellEjectPoint.right * Random.Range(2f, 4f), ForceMode.Impulse);
                    rb.AddTorque(Random.insideUnitSphere * 10f);
                }
                Destroy(shell, 3f);
            }
        }

        /// <summary>
        /// Apply camera recoil.
        /// </summary>
        protected virtual void ApplyRecoil()
        {
            if (_mouseLook != null)
            {
                float recoil = _weaponData.recoilAmount + Random.Range(-_weaponData.recoilVariance, _weaponData.recoilVariance);
                _mouseLook.ApplyRecoil(recoil);
            }
        }
        #endregion

        #region Ammo Management
        /// <summary>
        /// Add ammo to reserve.
        /// </summary>
        public void AddAmmo(int amount)
        {
            _reserveAmmo = Mathf.Min(_reserveAmmo + amount, _weaponData.maxReserveAmmo);
        }

        /// <summary>
        /// Check if weapon needs reload.
        /// </summary>
        public bool NeedsReload()
        {
            return _currentAmmo < _weaponData.magazineSize && _reserveAmmo > 0;
        }
        #endregion
    }
}