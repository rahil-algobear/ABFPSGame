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
        public DamageType damageType = DamageType.Ballistic;
        public float headshotMultiplier = 2f;
        public float range = 100f;
        public float maxRange = 100f;
        public bool hasDamageFalloff = true;
        public AnimationCurve damageFalloffCurve = AnimationCurve.Linear(0f, 1f, 1f, 0.5f);

        [Header("Fire Rate")]
        public float fireRate = 0.1f; // Time between shots
        public bool isAutomatic = true;

        [Header("Ammo")]
        public int magazineSize = 30;
        public int maxReserveAmmo = 120;
        public float reloadTime = 2f;

        [Header("Accuracy")]
        public float baseSpread = 0.5f;
        public float maxSpread = 5f;
        public float spreadIncrease = 0.3f;
        public float spreadIncreasePerShot = 0.3f;
        public float spreadRecovery = 2f;
        public float spreadDecreaseRate = 2f;
        public int pelletsPerShot = 1; // For shotguns

        [Header("Recoil")]
        public float recoilAmount = 1.5f;
        public float recoilVariance = 0.3f;
        public Vector2 recoilMin = new Vector2(-0.5f, 1f);
        public Vector2 recoilMax = new Vector2(0.5f, 2f);
        public float recoilRecoverySpeed = 5f;

        [Header("Effects")]
        public GameObject muzzleFlashPrefab;
        public GameObject bulletImpactPrefab;
        public GameObject shellCasingPrefab;

        [Header("Audio")]
        public AudioClip fireSound;
        public AudioClip reloadSound;
        public AudioClip emptySound;

        /// <summary>
        /// Calculate damage at a given distance.
        /// </summary>
        /// <param name="distance">Distance to target</param>
        /// <returns>Damage value</returns>
        public float GetDamageAtDistance(float distance)
        {
            if (!hasDamageFalloff || distance <= 0f)
            {
                return baseDamage;
            }

            float normalizedDistance = Mathf.Clamp01(distance / maxRange);
            float falloffMultiplier = damageFalloffCurve.Evaluate(normalizedDistance);
            return baseDamage * falloffMultiplier;
        }
    }
}