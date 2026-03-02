using UnityEngine;

namespace Game.Weapons
{
    /// <summary>
    /// ScriptableObject containing weapon statistics and properties.
    /// </summary>
    [CreateAssetMenu(fileName = "New Weapon Data", menuName = "Weapons/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        [Header("Basic Info")]
        public string weaponName = "Weapon";
        public Sprite weaponIcon;

        [Header("Damage")]
        public float damage = 25f;
        public float range = 100f;
        public float fireRate = 0.1f;

        [Header("Ammo")]
        public int magazineSize = 30;
        public int maxAmmo = 300;
        public float reloadTime = 2f;

        [Header("Recoil & Spread")]
        public float recoil = 1f;
        public float spread = 0.05f;

        [Header("Audio")]
        public AudioClip fireSound;
        public AudioClip reloadSound;
        public AudioClip emptySound;

        // Property accessors for compatibility
        public int MagazineSize => magazineSize;
        public int MaxReserveAmmo => maxAmmo;
        public float ReloadTime => reloadTime;
        public float RecoilAmount => recoil;
        public float SpreadAmount => spread;
        public float Range => range;
        public float Damage => damage;
        public float FireRate => fireRate;
    }
}