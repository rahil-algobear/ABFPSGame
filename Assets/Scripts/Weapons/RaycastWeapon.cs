using UnityEngine;
using Game.Core;

namespace Game.Weapons
{
    /// <summary>
    /// Generic raycast-based weapon implementation.
    /// </summary>
    public class RaycastWeapon : WeaponBase
    {
        [Header("Fire Point")]
        [SerializeField] private Transform _firePoint;

        public override void Fire()
        {
            if (!CanFire) return;

            _currentAmmo--;
            _nextFireTime = Time.time + _weaponData.FireRate;

            Vector3 direction = _firePoint.forward + GetSpreadOffset();
            RaycastHit hit;

            if (PerformRaycast(_firePoint.position, direction, out hit))
            {
                IDamageable damageable = hit.collider.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(_weaponData.Damage, DamageSystem.DamageType.Bullet, hit.point);
                }
            }

            SpawnMuzzleFlash();
            ApplyRecoil();
        }
    }
}