using UnityEngine;
using Game.Weapons;

namespace Game.Pickups
{
    /// <summary>
    /// Ammo pickup that restores weapon ammo.
    /// </summary>
    public class AmmoPickup : PickupBase
    {
        [Header("Ammo Settings")]
        [SerializeField] private int _ammoAmount = 30;
        [SerializeField] private int _weaponIndex = -1; // -1 for current weapon

        protected override bool OnPickup(GameObject player)
        {
            WeaponManager weaponManager = player.GetComponentInChildren<WeaponManager>();
            if (weaponManager != null)
            {
                if (_weaponIndex < 0)
                {
                    weaponManager.AddAmmo(_ammoAmount);
                }
                else
                {
                    weaponManager.AddAmmoToWeapon(_weaponIndex, _ammoAmount);
                }
                return true;
            }
            return false;
        }
    }
}
