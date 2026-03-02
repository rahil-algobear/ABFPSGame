using UnityEngine;
using Game.Player;

namespace Game.Pickups
{
    /// <summary>
    /// Health pickup that restores player health.
    /// </summary>
    public class HealthPickup : PickupBase
    {
        [Header("Health Settings")]
        [SerializeField] private float _healAmount = 25f;

        protected override bool OnPickup(GameObject player)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                // Only pickup if player needs health
                if (playerHealth.CurrentHealth < playerHealth.MaxHealth)
                {
                    playerHealth.Heal(_healAmount);
                    return true;
                }
            }
            return false;
        }
    }
}
