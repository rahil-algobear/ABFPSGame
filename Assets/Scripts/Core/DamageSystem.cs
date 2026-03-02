using UnityEngine;

namespace Game.Core
{
    /// <summary>
    /// Centralized damage calculation system with multipliers and resistance.
    /// </summary>
    public static class DamageSystem
    {
        #region Damage Types
        public enum DamageType
        {
            Bullet,
            Plasma,
            Explosion,
            Melee
        }

        public enum HitLocation
        {
            Head,
            Body,
            Limb
        }
        #endregion

        #region Damage Multipliers
        private const float HEAD_MULTIPLIER = 2.0f;
        private const float BODY_MULTIPLIER = 1.0f;
        private const float LIMB_MULTIPLIER = 0.7f;
        #endregion

        #region Damage Calculation
        /// <summary>
        /// Calculates final damage based on base damage, hit location, and damage type.
        /// </summary>
        /// <param name="baseDamage">Base damage amount</param>
        /// <param name="hitLocation">Where the hit landed</param>
        /// <param name="damageType">Type of damage</param>
        /// <param name="distance">Distance from damage source (for falloff)</param>
        /// <param name="maxRange">Maximum effective range</param>
        /// <returns>Final calculated damage</returns>
        public static float CalculateDamage(float baseDamage, HitLocation hitLocation, DamageType damageType, float distance = 0f, float maxRange = 100f)
        {
            float damage = baseDamage;

            // Apply hit location multiplier
            damage *= GetLocationMultiplier(hitLocation);

            // Apply distance falloff for certain damage types
            if (damageType == DamageType.Bullet || damageType == DamageType.Plasma)
            {
                damage *= CalculateDistanceFalloff(distance, maxRange);
            }

            return damage;
        }

        /// <summary>
        /// Gets the damage multiplier for a specific hit location.
        /// </summary>
        /// <param name="location">Hit location</param>
        /// <returns>Damage multiplier</returns>
        public static float GetLocationMultiplier(HitLocation location)
        {
            switch (location)
            {
                case HitLocation.Head:
                    return HEAD_MULTIPLIER;
                case HitLocation.Body:
                    return BODY_MULTIPLIER;
                case HitLocation.Limb:
                    return LIMB_MULTIPLIER;
                default:
                    return BODY_MULTIPLIER;
            }
        }

        /// <summary>
        /// Calculates damage falloff based on distance.
        /// </summary>
        /// <param name="distance">Distance from target</param>
        /// <param name="maxRange">Maximum effective range</param>
        /// <returns>Falloff multiplier (0-1)</returns>
        public static float CalculateDistanceFalloff(float distance, float maxRange)
        {
            if (distance <= 0f || maxRange <= 0f)
                return 1f;

            float falloff = 1f - (distance / maxRange);
            return Mathf.Clamp01(falloff);
        }

        /// <summary>
        /// Determines hit location based on collider tag or name.
        /// </summary>
        /// <param name="collider">Hit collider</param>
        /// <returns>Hit location</returns>
        public static HitLocation DetermineHitLocation(Collider collider)
        {
            if (collider == null)
                return HitLocation.Body;

            string tag = collider.tag.ToLower();
            string name = collider.name.ToLower();

            if (tag.Contains("head") || name.Contains("head"))
                return HitLocation.Head;
            else if (tag.Contains("limb") || name.Contains("arm") || name.Contains("leg"))
                return HitLocation.Limb;
            else
                return HitLocation.Body;
        }
        #endregion

        #region Explosion Damage
        /// <summary>
        /// Calculates explosion damage with radius falloff.
        /// </summary>
        /// <param name="baseDamage">Base explosion damage</param>
        /// <param name="distance">Distance from explosion center</param>
        /// <param name="explosionRadius">Explosion radius</param>
        /// <returns>Final damage amount</returns>
        public static float CalculateExplosionDamage(float baseDamage, float distance, float explosionRadius)
        {
            if (distance >= explosionRadius)
                return 0f;

            float falloff = 1f - (distance / explosionRadius);
            return baseDamage * falloff;
        }

        /// <summary>
        /// Applies explosion damage to all objects within radius.
        /// </summary>
        /// <param name="position">Explosion center</param>
        /// <param name="radius">Explosion radius</param>
        /// <param name="damage">Base damage</param>
        /// <param name="damageType">Type of damage</param>
        public static void ApplyExplosionDamage(Vector3 position, float radius, float damage, DamageType damageType)
        {
            Collider[] colliders = Physics.OverlapSphere(position, radius);

            foreach (Collider col in colliders)
            {
                // Check if object can take damage
                IDamageable damageable = col.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    float distance = Vector3.Distance(position, col.transform.position);
                    float finalDamage = CalculateExplosionDamage(damage, distance, radius);

                    if (finalDamage > 0f)
                    {
                        damageable.TakeDamage(finalDamage, damageType, position);
                    }
                }

                // Apply physics force
                Rigidbody rb = col.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddExplosionForce(damage * 10f, position, radius);
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// Interface for objects that can take damage.
    /// </summary>
    public interface IDamageable
    {
        void TakeDamage(float damage, DamageSystem.DamageType damageType, Vector3 hitPoint);
        void Die();
    }
}