using UnityEngine;
using Game.Core;

namespace Game.Weapons
{
    /// <summary>
    /// Automatic plasma rifle with energy projectiles.
    /// </summary>
    public class PlasmaRifle : WeaponBase
    {
        [Header("Plasma Settings")]
        [SerializeField] private GameObject _plasmaProjectilePrefab;
        [SerializeField] private float _projectileSpeed = 50f;
        [SerializeField] private float _projectileLifetime = 3f;

        /// <summary>
        /// Fires the plasma rifle (automatic).
        /// </summary>
        public override void Fire()
        {
            // Check if can fire
            if (_isReloading || Time.time < _nextFireTime)
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
            _currentAmmo--;
            _nextFireTime = Time.time + _weaponData.fireRate;

            // Effects
            PlayMuzzleFlash();
            PlayFireSound();

            // Fire plasma projectile
            FirePlasmaProjectile();

            // Recoil
            IncreaseSpread();
            ApplyRecoil();
        }

        /// <summary>
        /// Reloads the plasma rifle.
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
        /// Fires a plasma projectile.
        /// </summary>
        private void FirePlasmaProjectile()
        {
            if (_plasmaProjectilePrefab == null || _muzzlePoint == null)
            {
                // Fallback to raycast if no projectile prefab
                FireRaycast();
                return;
            }

            Vector3 direction = _playerCamera.transform.forward;
            direction = ApplySpread(direction);

            GameObject projectile = Instantiate(
                _plasmaProjectilePrefab,
                _muzzlePoint.position,
                Quaternion.LookRotation(direction)
            );

            // Add velocity to projectile
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = direction * _projectileSpeed;
            }

            // Setup projectile damage
            PlasmaProjectile plasma = projectile.GetComponent<PlasmaProjectile>();
            if (plasma != null)
            {
                plasma.Initialize(_weaponData.damage, _weaponData.damageType, _weaponData.range);
            }

            Destroy(projectile, _projectileLifetime);
        }

        /// <summary>
        /// Fallback raycast firing method.
        /// </summary>
        private void FireRaycast()
        {
            Vector3 direction = _playerCamera.transform.forward;
            direction = ApplySpread(direction);

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
                float recoil = _weaponData.recoilAmount * 0.5f; // Plasma rifle has less recoil
                mouseLook.ApplyRecoil(recoil);
            }
        }
    }

    /// <summary>
    /// Plasma projectile component.
    /// </summary>
    public class PlasmaProjectile : MonoBehaviour
    {
        private float _damage;
        private DamageSystem.DamageType _damageType;
        private float _maxRange;
        private Vector3 _startPosition;

        [SerializeField] private GameObject _impactEffectPrefab;

        private void Start()
        {
            _startPosition = transform.position;
        }

        private void Update()
        {
            // Check if exceeded max range
            if (Vector3.Distance(_startPosition, transform.position) > _maxRange)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Initializes the projectile with damage values.
        /// </summary>
        public void Initialize(float damage, DamageSystem.DamageType damageType, float maxRange)
        {
            _damage = damage;
            _damageType = damageType;
            _maxRange = maxRange;
        }

        private void OnTriggerEnter(Collider other)
        {
            // Apply damage
            Game.Core.IDamageable damageable = other.GetComponent<Game.Core.IDamageable>();
            if (damageable != null)
            {
                float distance = Vector3.Distance(_startPosition, transform.position);
                float finalDamage = DamageSystem.CalculateDamage(
                    _damage,
                    DamageSystem.DetermineHitLocation(other),
                    _damageType,
                    distance,
                    _maxRange
                );

                damageable.TakeDamage(finalDamage, _damageType, transform.position);
            }

            // Create impact effect
            if (_impactEffectPrefab != null)
            {
                Instantiate(_impactEffectPrefab, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}