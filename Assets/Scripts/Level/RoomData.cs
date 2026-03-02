using UnityEngine;

namespace Game.Level
{
    /// <summary>
    /// ScriptableObject for room configuration data.
    /// </summary>
    [CreateAssetMenu(fileName = "New Room Data", menuName = "Game/Room Data")]
    public class RoomData : ScriptableObject
    {
        #region Room Info
        [Header("Room Info")]
        public string RoomName = "Room";
        public Vector3 RoomSize = new Vector3(10, 4, 10);
        #endregion

        #region Spawning
        [Header("Spawning")]
        public bool CanSpawnEnemies = true;
        public int MinEnemies = 2;
        public int MaxEnemies = 5;
        #endregion

        #region Pickups
        [Header("Pickups")]
        public bool CanSpawnPickups = true;
        public GameObject[] PickupPrefabs;
        #endregion

        #region Lighting
        [Header("Lighting")]
        public Color LightColor = Color.white;
        public float LightIntensity = 1f;
        #endregion
    }
}
