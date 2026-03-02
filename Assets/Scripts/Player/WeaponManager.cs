using UnityEngine;
using Game.Weapons;
using Game.Core;
using System.Collections.Generic;

namespace Game.Player
{
    /// <summary>
    /// Manages weapon switching, ammo tracking, and weapon sway.
    /// </summary>
    public class WeaponManager : MonoBehaviour
    {
        #region Weapon Settings
        [Header("Weapon Settings")]
        [SerializeField] private List<WeaponBase> _weapons = new List<WeaponBase>();
        [SerializeField] private int _currentWeaponIndex = 0;
        [SerializeField] private Transform _weaponHolder;
        #endregion

        #region Weapon Sway Settings
        [Header("Weapon Sway")]
        [SerializeField] private bool _enableSway = true;
        [SerializeField] private float _swayAmount = 0.02f;
        [SerializeField] private float _maxSwayAmount = 0.06f;
        [SerializeField] private float _swaySmooth = 6f;
        #endregion

        #region Private Fields
        private WeaponBase _currentWeapon;
        private Vector3 _initialWeaponPosition;
        #endregion

        #region Events
        public delegate void WeaponChangedHandler(WeaponBase weapon);
        public event WeaponChangedHandler OnWeaponChanged;

        public delegate void AmmoChangedHandler(int currentAmmo, int reserveAmmo);
        public event AmmoChangedHandler OnAmmoChanged;
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            InitializeWeapons();
            EquipWeapon(_currentWeaponIndex);
        }

        private void Update()
        {
            if (GameManager.Instance.IsGameOver || GameManager.Instance.IsPaused)
                return;

            HandleWeaponSwitching();
            HandleWeaponSway();
            HandleWeaponInput();
        }
        #endregion

        #region Weapon Initialization
        /// <summary>
        /// Initializes all weapons in the weapon holder.
        /// </summary>
        private void InitializeWeapons()
        {
            if (_weaponHolder == null)
            {
                Debug.LogError("Weapon holder not assigned!");
                return;
            }

            // Get all weapons from weapon holder
            _weapons.Clear();
            foreach (Transform child in _weaponHolder)
            {
                WeaponBase weapon = child.GetComponent<WeaponBase>();
                if (weapon != null)
                {
                    _weapons.Add(weapon);
                    weapon.gameObject.SetActive(false);
                }
            }

            if (_weapons.Count == 0)
            {
                Debug.LogWarning("No weapons found in weapon holder!");
            }
        }
        #endregion

        #region Weapon Switching
        /// <summary>
        /// Handles weapon switching input.
        /// </summary>
        private void HandleWeaponSwitching()
        {
            // Number key switching
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                EquipWeapon(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                EquipWeapon(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                EquipWeapon(2);
            }

            // Mouse wheel switching
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll > 0f)
            {
                EquipNextWeapon();
            }
            else if (scroll < 0f)
            {
                EquipPreviousWeapon();
            }
        }

        /// <summary>
        /// Equips a weapon by index.
        /// </summary>
        /// <param name="index">Weapon index</param>
        public void EquipWeapon(int index)
        {
            if (index < 0 || index >= _weapons.Count)
                return;

            if (_currentWeapon != null && index == _currentWeaponIndex)
                return;

            // Deactivate current weapon
            if (_currentWeapon != null)
            {
                _currentWeapon.gameObject.SetActive(false);
            }

            // Activate new weapon
            _currentWeaponIndex = index;
            _currentWeapon = _weapons[_currentWeaponIndex];
            _currentWeapon.gameObject.SetActive(true);
            _initialWeaponPosition = _currentWeapon.transform.localPosition;

            OnWeaponChanged?.Invoke(_currentWeapon);
            UpdateAmmoDisplay();
        }

        /// <summary>
        /// Equips the next weapon in the list.
        /// </summary>
        public void EquipNextWeapon()
        {
            int nextIndex = (_currentWeaponIndex + 1) % _weapons.Count;
            EquipWeapon(nextIndex);
        }

        /// <summary>
        /// Equips the previous weapon in the list.
        /// </summary>
        public void EquipPreviousWeapon()
        {
            int prevIndex = (_currentWeaponIndex - 1 + _weapons.Count) % _weapons.Count;
            EquipWeapon(prevIndex);
        }
        #endregion

        #region Weapon Input
        /// <summary>
        /// Handles weapon firing and reloading input.
        /// </summary>
        private void HandleWeaponInput()
        {
            if (_currentWeapon == null)
                return;

            // Fire weapon
            if (Input.GetButton("Fire1"))
            {
                _currentWeapon.Fire();
                UpdateAmmoDisplay();
            }

            // Aim down sights
            if (Input.GetButton("Fire2"))
            {
                _currentWeapon.AimDownSights(true);
            }
            else
            {
                _currentWeapon.AimDownSights(false);
            }

            // Reload
            if (Input.GetKeyDown(KeyCode.R))
            {
                _currentWeapon.Reload();
                UpdateAmmoDisplay();
            }
        }
        #endregion

        #region Weapon Sway
        /// <summary>
        /// Applies weapon sway based on mouse movement.
        /// </summary>
        private void HandleWeaponSway()
        {
            if (!_enableSway || _currentWeapon == null)
                return;

            float mouseX = Input.GetAxis("Mouse X") * _swayAmount;
            float mouseY = Input.GetAxis("Mouse Y") * _swayAmount;

            mouseX = Mathf.Clamp(mouseX, -_maxSwayAmount, _maxSwayAmount);
            mouseY = Mathf.Clamp(mouseY, -_maxSwayAmount, _maxSwayAmount);

            Vector3 targetPosition = new Vector3(-mouseX, -mouseY, 0);
            _currentWeapon.transform.localPosition = Vector3.Lerp(
                _currentWeapon.transform.localPosition,
                _initialWeaponPosition + targetPosition,
                Time.deltaTime * _swaySmooth
            );
        }
        #endregion

        #region Ammo Management
        /// <summary>
        /// Updates the ammo display UI.
        /// </summary>
        private void UpdateAmmoDisplay()
        {
            if (_currentWeapon != null)
            {
                OnAmmoChanged?.Invoke(_currentWeapon.GetCurrentAmmo(), _currentWeapon.GetReserveAmmo());
            }
        }

        /// <summary>
        /// Adds ammo to the current weapon.
        /// </summary>
        /// <param name="amount">Amount of ammo to add</param>
        public void AddAmmo(int amount)
        {
            if (_currentWeapon != null)
            {
                _currentWeapon.AddReserveAmmo(amount);
                UpdateAmmoDisplay();
            }
        }

        /// <summary>
        /// Adds ammo to a specific weapon type.
        /// </summary>
        /// <param name="weaponName">Name of the weapon</param>
        /// <param name="amount">Amount of ammo to add</param>
        public void AddAmmoToWeapon(string weaponName, int amount)
        {
            foreach (WeaponBase weapon in _weapons)
            {
                if (weapon.name.Contains(weaponName))
                {
                    weapon.AddReserveAmmo(amount);
                    if (weapon == _currentWeapon)
                    {
                        UpdateAmmoDisplay();
                    }
                    break;
                }
            }
        }
        #endregion

        #region Public Getters
        public WeaponBase GetCurrentWeapon() => _currentWeapon;
        public int GetCurrentWeaponIndex() => _currentWeaponIndex;
        public int GetWeaponCount() => _weapons.Count;
        #endregion
    }
}