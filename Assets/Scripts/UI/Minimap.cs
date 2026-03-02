using UnityEngine;

namespace Game.UI
{
    /// <summary>
    /// Minimap camera that follows the player.
    /// </summary>
    public class Minimap : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Transform _player;
        [SerializeField] private float _height = 50f;
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
            if (_player == null) return;

            Vector3 newPosition = _player.position;
            newPosition.y = _player.position.y + _height;
            transform.position = newPosition;

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
