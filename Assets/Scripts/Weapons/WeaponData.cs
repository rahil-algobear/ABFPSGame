using UnityEngine;

namespace Game.Weapons
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        [Header("Basic Info")]
        public string weaponName;
        public WeaponType weaponType;

        [Header("Damage")]
        public float damage = 25f;
        public float headshotMultiplier = 2f;

        [Header("Fire Rate")]
        public float fireRate = 10f;
        public FireMode fireMode = FireMode.Automatic;

        [Header("Ammo")]
        public int magazineSize = 30;
        public int maxAmmo = 300;
        public float reloadTime = 2f;

        [Header("Accuracy")]
        public float baseSpread = 0.1f;
        public float maxSpread = 2f;
        public float spreadIncreasePerShot = 0.05f;
        public float spreadDecreaseRate = 2f;

        [Header("Recoil")]
        public float recoilAmount = 0.5f;
        public float recoilRecoverySpeed = 5f;

        [Header("Range")]
        public float maxRange = 100f;

        public enum WeaponType
        {
            Pistol,
            Rifle,
            Shotgun,
            Sniper
        }

        public enum FireMode
        {
            SemiAutomatic,
            Automatic,
            Burst
        }
    }
}