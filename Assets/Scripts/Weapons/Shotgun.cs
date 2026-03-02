using UnityEngine;
using Game.Core;

namespace Game.Weapons
{
    /// <summary>
    /// Pump-action shotgun with spread pellets.
    /// </summary>
    public class Shotgun : WeaponBase
    {
        #region Effects
        [Header("Effects")]
        [SerializeField] private ParticleSystem _muzzleFlash;
        [SerializeField] private GameObject _impactEffect;
        [SerializeField] private Transform _firePoint;
        #endregion

        #region Audio
        [Header("Audio")]
        [SerializeField] private AudioSource _audioSource;
        #endregion

        #region Shotgun Settings
        [Header("Shotgun Settings")]
        [SerializeField] private int _pelletsPerShot = 8;
        [SerializeField] private float _pelletSpread = 0.15f;
        #endregion

        #region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            if (_audioSource == null)
            {
                _audioSource = GetComponent<AudioSource>();
            }

            if (_firePoint == null)
            {
                _firePoint = transform;
            }
        }

        protected override void Update()
        {
            base.Update();

            // Semi-automatic fire
            if (Input.GetButtonDown("Fire1") && CanFire)
            {
                Fire();
            }

            // Reload
            if (Input.GetKeyDown(KeyCode.R))
            {
                Reload();
            }
        }
        #endregion

        #region Fire
        public override void Fire()
        {
            if (!CanFire) return;

            _currentAmmo--;
            _nextFireTime = Time.time + _weaponData.FireRate;

            // Play effects
            if (_muzzleFlash != null)
            {
                _muzzleFlash.Play();
            }

            if (_audioSource != null && _weaponData.fireSound != null)
            {
                _audioSource.PlayOneShot(_weaponData.fireSound);
            }

            // Fire multiple pellets
            for (int i = 0; i < _pelletsPerShot; i++)
            {
                FirePellet();
            }

            // Apply recoil
            ApplyRecoil();
        }

        private void FirePellet()
        {
            Vector3 spread = new Vector3(
                Random.Range(-_pelletSpread, _pelletSpread),
                Random.Range(-_pelletSpread, _pelletSpread),
                0f
            );

            Vector3 direction = _firePoint.forward + spread;
            RaycastHit hit;

            if (PerformRaycast(_firePoint.position, direction, out hit))
            {
                // Apply damage
                IDamageable damageable = hit.collider.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    DamageSystem.HitLocation hitLocation = DamageSystem.DetermineHitLocation(hit.collider);
                    float pelletDamage = _weaponData.Damage / _pelletsPerShot;
                    float damage = DamageSystem.CalculateDamage(
                        pelletDamage,
                        hitLocation,
                        DamageSystem.DamageType.Bullet,
                        hit.distance,
                        _weaponData.Range
                    );
                    damageable.TakeDamage(damage, DamageSystem.DamageType.Bullet, hit.point);
                }

                // Spawn impact effect
                if (_impactEffect != null)
                {
                    GameObject impact = Instantiate(_impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(impact, 2f);
                }
            }
        }

        protected override void ApplyRecoil()
        {
            if (_mouseLook != null && _weaponData != null)
            {
                _mouseLook.ApplyRecoil(_weaponData.RecoilAmount * 1.5f, _weaponData.RecoilAmount * 0.75f);
            }
        }
        #endregion
    }
}