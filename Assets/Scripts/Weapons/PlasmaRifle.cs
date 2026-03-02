using UnityEngine;
using Game.Core;

namespace Game.Weapons
{
    /// <summary>
    /// Automatic plasma rifle with energy projectiles.
    /// </summary>
    public class PlasmaRifle : WeaponBase
    {
        #region Effects
        [Header("Effects")]
        [SerializeField] private ParticleSystem _muzzleFlash;
        [SerializeField] private GameObject _impactEffect;
        [SerializeField] private Transform _firePoint;
        [SerializeField] private LineRenderer _beamEffect;
        #endregion

        #region Audio
        [Header("Audio")]
        [SerializeField] private AudioSource _audioSource;
        #endregion

        #region Plasma Settings
        [Header("Plasma Settings")]
        [SerializeField] private float _beamDuration = 0.1f;
        [SerializeField] private Color _beamColor = Color.cyan;
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

            if (_beamEffect != null)
            {
                _beamEffect.enabled = false;
            }
        }

        protected override void Update()
        {
            base.Update();

            // Automatic fire
            if (Input.GetButton("Fire1") && CanFire)
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

            // Perform raycast
            Vector3 direction = _firePoint.forward + GetSpreadOffset();
            RaycastHit hit;

            if (PerformRaycast(_firePoint.position, direction, out hit))
            {
                // Apply damage
                IDamageable damageable = hit.collider.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    DamageSystem.HitLocation hitLocation = DamageSystem.DetermineHitLocation(hit.collider);
                    float damage = DamageSystem.CalculateDamage(
                        _weaponData.Damage,
                        hitLocation,
                        DamageSystem.DamageType.Plasma,
                        hit.distance,
                        _weaponData.Range
                    );
                    damageable.TakeDamage(damage, DamageSystem.DamageType.Plasma, hit.point);
                }

                // Spawn impact effect
                if (_impactEffect != null)
                {
                    GameObject impact = Instantiate(_impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(impact, 2f);
                }

                // Show beam effect
                if (_beamEffect != null)
                {
                    StartCoroutine(ShowBeam(hit.point));
                }
            }
            else
            {
                // Show beam to max range
                if (_beamEffect != null)
                {
                    StartCoroutine(ShowBeam(_firePoint.position + direction * _weaponData.Range));
                }
            }

            // Apply recoil
            ApplyRecoil();
        }

        protected override void ApplyRecoil()
        {
            if (_mouseLook != null && _weaponData != null)
            {
                _mouseLook.ApplyRecoil(_weaponData.RecoilAmount * 0.7f, _weaponData.RecoilAmount * 0.35f);
            }
        }

        private System.Collections.IEnumerator ShowBeam(Vector3 endPoint)
        {
            _beamEffect.enabled = true;
            _beamEffect.SetPosition(0, _firePoint.position);
            _beamEffect.SetPosition(1, endPoint);

            yield return new WaitForSeconds(_beamDuration);

            _beamEffect.enabled = false;
        }
        #endregion
    }
}