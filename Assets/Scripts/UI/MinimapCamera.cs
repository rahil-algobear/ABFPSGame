using UnityEngine;

namespace Game.UI
{
    /// <summary>
    /// Controls the minimap camera that follows the player.
    /// </summary>
    public class MinimapCamera : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Transform _player;
        [SerializeField] private float _height = 20f;
        [SerializeField] private bool _rotateWithPlayer = false;

        private void Start()
        {
            if (_player == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                {
                    _player = playerObj.transform;
                }
            }
        }

        private void LateUpdate()
        {
            if (_player == null)
                return;

            // Follow player position
            Vector3 newPosition = _player.position;
            newPosition.y = _player.position.y + _height;
            transform.position = newPosition;

            // Optionally rotate with player
            if (_rotateWithPlayer)
            {
                transform.rotation = Quaternion.Euler(90f, _player.eulerAngles.y, 0f);
            }
            else
            {
                transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            }
        }
    }
}