using UnityEngine;
using Game.Core;

namespace Game.Weapons
{
    /// <summary>
    /// ScriptableObject containing weapon statistics and configuration.
    /// </summary>
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Game/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        [Header("Weapon Info")]
        public string weaponName = "Weapon";
        public Sprite weaponIcon;

        [Header("Damage")]
        public float damage = 25f;
        public float baseDamage = 25f;
        public DamageSystem.DamageType damageType = DamageSystem.DamageType.Bullet;
        public float headshotMultiplier = 2f;
        public float range = 100f;
        public float maxRange = 100f;
        public bool hasDamageFalloff = true;
        public int pelletsPerShot = 1;

        [Header("Fire Rate")]
        public float fireRate = 0.1f;
        public bool isAutomatic = false;

        [Header("Ammo")]
        public int magazineSize = 30;
        public int maxReserveAmmo = 120;

        [Header("Reload")]
        public float reloadTime = 2f;

        [Header("Accuracy")]
        public float baseSpread = 0.5f;
        public float maxSpread = 5f;
        public float spreadIncrease = 0.2f;
        public float spreadRecovery = 2f;

        [Header("Recoil")]
        public float recoilAmount = 1f;
        public float recoilVariance = 0.2f;

        [Header("Audio")]
        public AudioClip fireSound;
        public AudioClip reloadSound;
        public AudioClip emptySound;
    }
}