using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Game.UI
{
    /// <summary>
    /// Shows directional damage indicators on the screen.
    /// </summary>
    public class DamageIndicator : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Image _indicatorImage;
        [SerializeField] private float _displayDuration = 1f;
        [SerializeField] private Color _indicatorColor = new Color(1f, 0f, 0f, 0.5f);

        private Transform _player;
        private Camera _playerCamera;

        private void Start()
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                _player = playerObj.transform;
                _playerCamera = Camera.main;
            }

            if (_indicatorImage != null)
            {
                _indicatorImage.color = Color.clear;
            }
        }

        /// <summary>
        /// Shows a damage indicator from a specific direction.
        /// </summary>
        public void ShowIndicator(Vector3 damageSource)
        {
            if (_player == null || _indicatorImage == null)
                return;

            StartCoroutine(ShowIndicatorCoroutine(damageSource));
        }

        private IEnumerator ShowIndicatorCoroutine(Vector3 damageSource)
        {
            // Calculate direction to damage source
            Vector3 direction = (damageSource - _player.position).normalized;
            float angle = Vector3.SignedAngle(_player.forward, direction, Vector3.up);

            // Rotate indicator
            _indicatorImage.rectTransform.rotation = Quaternion.Euler(0f, 0f, -angle);

            // Fade in
            _indicatorImage.color = _indicatorColor;

            // Wait
            float elapsed = 0f;
            while (elapsed < _displayDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(_indicatorColor.a, 0f, elapsed / _displayDuration);
                _indicatorImage.color = new Color(_indicatorColor.r, _indicatorColor.g, _indicatorColor.b, alpha);
                yield return null;
            }

            _indicatorImage.color = Color.clear;
        }
    }
}